using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HenryMeds.Models;

namespace HenryMeds.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;

  private readonly IAppointmentsRepository db;

  public UserController(ILogger<UserController> logger)
  {
    _logger = logger;

    AppointmentsContext ctx = new AppointmentsContext();
    db  = new AppointmentsRepository(ctx);
  }

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
