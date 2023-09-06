using Microsoft.EntityFrameworkCore;

namespace HenryMeds.Models;

/// <summary>
/// An instantiation of the repository used to interface with the <see cref="AppointmentsContext"/>.
/// </summary>
public class AppointmentsRepository : IAppointmentsRepository
{
  private readonly AppointmentsContext _context;

  /// <summary>
  /// Instantiates a new instance of <see cref="AppointmentsRepository"/>.
  /// </summary>
  /// <param name="context"></param>
  public AppointmentsRepository(AppointmentsContext context)
    => _context = context;

  /// <inheritdoc/>
  public Availability? CreateAvailability(AvailabilityCreateDTO availability)
  {
    if (_context.Availabilities == null) {
      return null;
    }

    Availability newAvailability = availability.FromAvailabilityCreateDTO();

    // Right now, this isn't verifying that the user is a provider. A good addition would be to
    // first fetch the user, then validate that they are a provider. This could be a restriction
    // set up on insertion in the database itself instead.

    var entry = _context.Availabilities.Add(newAvailability);
    _context.SaveChanges();

    return entry.Entity;
  }

  /// <inheritdoc/>
  public Reservation? CreateReservation(ReservationCreateDTO reservation)
  {
    if (_context.Reservations == null || _context.Availabilities == null) {
      return null;
    }

    // This is not currently checking for a 15-minute resolution. This is in the interest of time-
    // boxing the project.
    //
    // Ideally, this would be a filter when inserting a rule that lives in PostGres, rather than
    // a hard-coded rule here. That would remove the back-and-forth between the DB that we do in
    // order to validate the data prior to insertion.
    //
    // Right now this just blindly inserts a reservation without checking for over-booking. This is
    // a desired feature, but not implemented here in favor of timeboxing the project. This could be
    // another limit when adding the reservation; if one already exists at this time, then do not
    // book.
    Availability? availability = _context.Availabilities
      .Where(
          (availability) =>
            availability.Start < reservation.ReservationTime
            && availability.End > reservation.ReservationTime
          )
      .First();

    if (availability == null)
    {
      return null;
    }

    Reservation newReservation = reservation.FromReservationCreateDTO();

    var entry = _context.Reservations.Add(newReservation);
    _context.SaveChanges();

    return entry.Entity;
  }

  /// <inheritdoc/>
  public User? CreateUser(UserCreateDTO user)
  {
    if (_context.Users == null) {
      return null;
    }

    User newUser = user.FromUserCreateDTO();

    var entry = _context.Users.Add(newUser);
    _context.SaveChanges();

    return entry.Entity;
  }
  
  /// <inheritdoc/>
  public IEnumerable<Availability> GetAvailabilitiesByProvider(Guid providerID)
  {
    if (_context.Availabilities == null) {
      return new List<Availability>();
    }

    IEnumerable<Availability> availabilities = _context.Availabilities
      .Where((Availability a) => a.ProviderId == providerID)
      .Include((Availability a) => a.Provider);

    return availabilities;
  }

  /// <inheritdoc/>
  public Reservation? UpdateReservation(
    Reservation reservation,
    ReservationUpdateDTO reservationDTO
  )
  {
    if (_context.Reservations == null) {
      return null;
    }

    var entry = _context.Entry(reservation);
    if (entry == null)
    {
      // Ideally, we'd return something discrete here so that we can differentiate between a DB that
      // failed to load vs failing to find an ID.
      return null;
    }

    entry.CurrentValues.SetValues(reservationDTO);
    _context.SaveChanges();

    return entry.Entity;
  }

  /// <inheritdoc/>
  public Reservation? GetReservation(Guid reservationID)
  {
    if (_context.Reservations == null)
    {
      return null;
    }

    Reservation? foundReservation = _context.Reservations.Find(reservationID);
    return foundReservation;
  }
}
