using Microsoft.AspNetCore.Mvc;
using HenryMeds.Models;

namespace HenryMeds.Controllers;

/// <summary>
/// A controller used to interface with <see cref="Availability"/> items.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AvailabilityController : ControllerBase
{
  private readonly ILogger<AvailabilityController> _logger;

  private readonly IAppointmentsRepository db;

  /// <summary>
  /// Instantiates a new instance of <see cref="AvailabilityController"/>.
  /// </summary>
  /// <param name="logger">The associated logger. This should be DI'd.</param>
  public AvailabilityController(ILogger<AvailabilityController> logger)
  {
    _logger = logger;

    AppointmentsContext ctx = new AppointmentsContext();
    db  = new AppointmentsRepository(ctx);
  }

  /// <summary>
  /// Creates an availability for a given provider.
  /// </summary>
  /// <param name="availability">The availability to create for the provider.</param>
  /// <returns>
  ///     - (200) OK, the availability was created. This returns a copy.
  ///     - (500) There was an error.
  /// </returns>
  [HttpPost()]
  public IActionResult SetAvailability(
    AvailabilityCreateDTO availability
  )
  {
    _logger.Log(LogLevel.Trace, $"Received request to set an availability {9999}");

    Availability? createdAvailability;
    try
    {
      createdAvailability = db.CreateAvailability(availability);
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
  /// <returns>
  ///     - (200) OK, the availabilities were found. This returns a copy.
  ///     - (500) There was an error.
  /// </returns>
  [HttpGet("{providerID}")]
  public IActionResult GetAvailabilities(string providerID)
  {
    _logger.Log(LogLevel.Trace, $"Received request to get availabilities for {9999}");

    bool canGetGuid = Guid.TryParse(providerID, out Guid idAsGuid);
    if (!canGetGuid)
    {
      return ValidationProblem("Provided ID is not a valid GUID");
    }

    IEnumerable<Availability> availabilities;
    try
    {
      availabilities = db.GetAvailabilitiesByProvider(idAsGuid);
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

    return new OkObjectResult(availabilities);
  }
}
