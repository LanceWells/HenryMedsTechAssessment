using Microsoft.AspNetCore.Mvc;
using HenryMeds.Models;

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

    AppointmentsContext ctx = new AppointmentsContext();
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
  [HttpPost()]
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
}
