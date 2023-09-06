using Microsoft.AspNetCore.Mvc;
using HenryMeds.Models;

namespace HenryMeds.Controllers;

/// <summary>
/// A controller used to interface with <see cref="Reservation"/> items.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
  private readonly ILogger<ReservationController> _logger;

  private readonly IAppointmentsRepository db;

  /// <summary>
  /// Instantiates a new instance of <see cref="ReservationController"/>.
  /// </summary>
  /// <param name="logger">The associated logger. This should be DI'd.</param>
  public ReservationController(ILogger<ReservationController> logger)
  {
    _logger = logger;

    AppointmentsContext ctx = new AppointmentsContext();
    db  = new AppointmentsRepository(ctx);
  }

  /// <summary>
  /// Books an appointment. This requires the ID for a given user, provider, and a time slot. This
  /// can return an error code if the given time slot is not available.
  /// </summary>
  /// <param name="reservation">Information regarding the reservation to book.</param>
  /// <returns>
  ///     - (200) OK, the appointment was booked. This returns a copy.
  ///     - (500) There was an error.
  /// </returns>
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

  /// <summary>
  /// Updates a given appointment. Currently this only updates the confirmation status of the
  /// appointment. If this changes, the function name should reflect that update.
  /// </summary>
  /// <param name="reservationID">The ID for the reservation to confirm.</param>
  /// <param name="reservationUpdate">
  /// Generic mutable information for the reservation to update. Note that this only includes the
  /// confirmation status at present.
  /// </param>
  /// <returns>
  ///     - (200) OK, the reservation was updated. This returns a copy.
  ///     - (404) The reservation was not found.
  ///     - (409) The reservation was expired and cannot be updated.
  ///     - (500) There was an error.
  /// </returns>
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
