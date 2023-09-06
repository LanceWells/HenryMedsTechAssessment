using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HenryMeds.Models;

namespace HenryMeds.Controllers;

[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
  private readonly ILogger<ReservationController> _logger;

  private readonly IAppointmentsRepository db;

  public ReservationController(ILogger<ReservationController> logger)
  {
    _logger = logger;

    AppointmentsContext ctx = new AppointmentsContext();
    db  = new AppointmentsRepository(ctx);
  }

  [HttpPost]
  public IActionResult BookAppointment(ReservationCreateDTO reservation)
  {
    _logger.Log(LogLevel.Trace, $"Received request to book an appointment {9999}");

    Reservation? createdReservation;
    try
    {
      createdReservation = db.CreateReservation(reservation);
    }
    catch (Exception e)
    {
      _logger.Log(LogLevel.Error, "Failed to create reservation", e);
      return StatusCode(500); 
    }

    if (reservation == null)
    {
      _logger.Log(LogLevel.Error, "Failed to create reservation");
      return StatusCode(500); 
    }

    return new OkObjectResult(createdReservation);
  }

  [HttpPatch("{reservationID}")]
  public IActionResult UpdateAppointmentConfirmation(
    string reservationID,
    ReservationUpdateDTO reservationUpdate
  )
  {
    _logger.Log(LogLevel.Trace, $"Received request to confirm an appointment {9999}");

    bool canGetGuid = Guid.TryParse(reservationID, out Guid idAsGuid);
    if (!canGetGuid)
    {
      return ValidationProblem("Provided ID is not a valid GUID");
    }

    Reservation? foundReservation;
    try
    {
      foundReservation = db.GetReservation(idAsGuid);
    }
    catch (Exception e)
    {
      _logger.Log(LogLevel.Error, "Failed to confirm reservation", e);
      return StatusCode(500); 
    }

    if (foundReservation == null)
    {
      _logger.Log(LogLevel.Error, "Failed to find reservation to confirm");
      return NotFound();
    }

    // Techincally, we could be trying to update some other value, but for now we're treating the
    // expiration date like a soft delete, which prohibits any further updates to the row.
    if (foundReservation.Expiration < DateTime.Now)
    {
      _logger.Log(LogLevel.Debug, "Attempted to confirm an expired reservation");
      return Conflict($"Reservation with id {reservationID} has already expired");
    }

    Reservation? updatedReservation;
    try
    {
      updatedReservation = db.UpdateReservation(foundReservation, reservationUpdate);
    }
    catch (Exception e)
    {
      _logger.Log(LogLevel.Error, "Failed to confirm reservation", e);
      return StatusCode(500); 
    }

    if (updatedReservation == null)
    {
      _logger.Log(LogLevel.Error, "Failed to find reservation to confirm");
      return NotFound();
    }

    return new OkObjectResult(updatedReservation);
  }
}
