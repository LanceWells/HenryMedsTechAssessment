using Microsoft.AspNetCore.Mvc;
using HenryMeds.Models;
using HenryMeds.Util;
using System.Text.Json;

namespace HenryMeds.Controllers;

/// <summary>
/// A controller used to interface with <see cref="User"/> items.
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;

  private readonly IAppointmentsRepository db;

  /// <summary>
  /// Instantiates a new instance of <see cref="UserController"/>.
  /// </summary>
  /// <param name="logger">The associated logger. This should be DI'd.</param>
  public UserController(ILogger<UserController> logger)
  {
    _logger = logger;

    AppointmentsInMemoryContext ctx = new AppointmentsInMemoryContext();
    db  = new AppointmentsRepository(ctx);
  }

  /// <summary>
  /// Creates a new user in the database.
  /// </summary>
  /// <param name="user">The user to be created.</param>
  /// <returns>
  ///     - (200) OK, the user was created. This returns a copy.
  ///     - (500) There was an error.
  /// </returns>
  [HttpPost]
  public IActionResult CreateUser(UserCreateDTO user)
  {
    _logger.Log(LogLevel.Trace, $"Received request to create a new user {9999}");

    User? createdUser;
    try
    {
      createdUser = db.CreateUser(user);
    }
    catch(Exception e)
    {
      _logger.Log(LogLevel.Error, "Failed to create a user", e);
      return StatusCode(500);
    }

    if (createdUser == null)
    {
      _logger.Log(LogLevel.Error, "Failed to create a user");
      return StatusCode(500);
    }

    return Created($"/{createdUser.Id}", createdUser);
  }

  /// <summary>
  /// Creates an availability for a given provider.
  /// </summary>
  /// <param name="availability">The availability to create for the provider.</param>
  /// <returns>
  ///     - (200) OK, the availability was created. This returns a copy.
  ///     - (400) A provided parameter was invalid.
  ///     - (500) There was an error.
  /// </returns>
  [HttpPost("{providerID}/availability")]
  public IActionResult SetAvailability(
    string providerID,
    AvailabilityCreateDTO availability
  )
  {
    _logger.Log(LogLevel.Trace, $"Received request to set an availability {JsonSerializer.Serialize(availability)}");

    bool canGetGuid = Guid.TryParse(providerID, out Guid idAsGuid);
    if (!canGetGuid)
    {
      return ValidationProblem("Provided ID is not a valid GUID");
    }

    // Check the date time increments here. Even though we could round the times off, we don't so
    // that the provided payload exactly matches what will be created in the database.
    DateTime startTime = DateTimeUtils.Round15Mintues(availability.Start);
    if (startTime.Ticks != availability.Start.Ticks) {
      string error = "Provided date time start is not a 15-minute increment.";
      _logger.Log(LogLevel.Error, error);
      return BadRequest(error);
    }

    DateTime endTime = DateTimeUtils.Round15Mintues(availability.End);
    if (endTime.Ticks != availability.End.Ticks) {
      string error = "Provided date time end is not a 15-minute increment.";
      _logger.Log(LogLevel.Error, error);
      return BadRequest(error);
    }

    // In a "real world" implementation, we could relegate this to an API layer that first resolves
    // user details, permissions, and authentication.
    User? user = db.GetUser(idAsGuid);
    if (user == null)
    {
      string error = $"Could not find a user with ID {idAsGuid}";
      _logger.Log(LogLevel.Error, error);
      return NotFound(error);
    }
    if (user.UserType != UserType.Provider)
    {
      string error = "User is not a provider";
      _logger.Log(LogLevel.Error, error);
      return Unauthorized(error);
    }

    Availability? createdAvailability;
    try
    {
      createdAvailability = db.CreateAvailability(availability, idAsGuid);
    }
    catch (Exception e)
    {
      _logger.Log(LogLevel.Error, "Failed to set an availability", e);
      return StatusCode(500);
    }

    if (createdAvailability == null)
    {
      _logger.Log(LogLevel.Error, "Failed to set an availability");
      return StatusCode(500);
    }

    return Created($"/{createdAvailability.Id}", createdAvailability);
  }

  /// <summary>
  /// Gets the list of availabilities for the given provider.
  /// </summary>
  /// <param name="providerID">The provider to fetch the availabilities for.</param>
  /// <param name="start">
  /// The starting time at which to search for availabilities. This finds availabilities that have
  /// started before this time.
  /// </param>
  /// <param name="end">
  /// The ending time at which to search for availabilities. This finds availabilities that have
  /// ended after this time.
  /// </param>
  /// <returns>
  ///     - (200) OK, the availabilities were found. This returns a copy.
  ///     - (500) There was an error.
  /// </returns>
  /// <remarks>
  /// The default behavior is that this endpoint does not return appointment slots. The idea is that
  /// the frontend portion of an application is able to determine what availabilities should look
  /// like to a user. This also reduces the overall size of the data being sent back to the
  /// frontend.
  /// 
  /// Alternatively, a user may request the range as timeslots using the <paramref name="asSlots"/>
  /// parameter.
  /// </remarks>
  [HttpGet("{providerID}/availability")]
  public IActionResult GetAvailabilities(
    string providerID,
    [FromQuery(Name = "start")] DateTime? start,
    [FromQuery(Name = "end")] DateTime? end,
    [FromQuery(Name = "asSlots")] bool asSlots
  )
  {
    _logger.Log(LogLevel.Trace, $"Received request to get availabilities for {providerID}.");

    bool canGetGuid = Guid.TryParse(providerID, out Guid idAsGuid);
    if (!canGetGuid)
    {
      return ValidationProblem("Provided ID is not a valid GUID");
    }

    IEnumerable<Availability> availabilities;
    try
    {
      availabilities = db.GetAvailabilitiesByProvider(idAsGuid, start, end);
    }
    catch (Exception e)
    {
      _logger.Log(LogLevel.Error, "Failed to get availabilities", e);
      return StatusCode(500); 
    }

    if (availabilities == null)
    {
      _logger.Log(LogLevel.Error, "Failed to get availabilities");
      return StatusCode(500); 
    }

    if (asSlots)
    {
      IEnumerable<DateTime> timeSlots = availabilities.SelectMany((availability) => {
        var range = availability.End - availability.Start;
        var theseTimeSlots = new List<DateTime>();
        for (TimeSpan i = TimeSpan.Zero; i < range; i += TimeSpan.FromMinutes(15))
        {
          theseTimeSlots.Add(availability.Start + i);
        }
        return theseTimeSlots;
      });

      return new OkObjectResult(timeSlots);
    }
    else
    {
      return new OkObjectResult(availabilities);
    }
  }
}
