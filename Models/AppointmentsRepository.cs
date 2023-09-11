using HenryMeds.Util;

namespace HenryMeds.Models;

/// <summary>
/// An instantiation of the repository used to interface with the <see cref="AppointmentsInMemoryContext"/>.
/// </summary>
public class AppointmentsRepository : IAppointmentsRepository
{
  private readonly AppointmentsInMemoryContext _context;

  /// <summary>
  /// Instantiates a new instance of <see cref="AppointmentsRepository"/>.
  /// </summary>
  /// <param name="context"></param>
  public AppointmentsRepository(AppointmentsInMemoryContext context)
    => _context = context;

  /// <inheritdoc/>
  public Availability? CreateAvailability(AvailabilityCreateDTO availability, Guid providerID)
  {
    if (_context.Availabilities == null || _context.Users == null) {
      return null;
    }

    Availability newAvailability = availability.FromAvailabilityCreateDTO(providerID);

    var entry = _context.Availabilities.Add(newAvailability);
    _context.SaveChanges();

    return entry.Entity;
  }

  /// <inheritdoc/>
  public Reservation? CreateReservation(ReservationCreateDTO reservation)
  {
    if (_context.Reservations == null || _context.Availabilities == null || _context.Users == null) {
      return null;
    }

    // Ideally, this would be a check when inserting a rule that lives in PostGres, rather than
    // a hard-coded rule here. That would remove the back-and-forth between the DB that we do in
    // order to validate the data prior to insertion. This is why this lives in the repository code,
    // rather than in the resolver. This isn't a check becuase we're using an in-memory database for
    // this version.
    //
    // This datetime comparison should be fine (it works in in-memory databases), as the actual
    // query being executed should check for value comparison rather than for object comparison.
    //
    // Note that this doesn't look for an "overlapping" time because appointments are at most 15
    // mintues long, and we only allow appointments to be submitted at 15-minute increments on the
    // hour.
    IQueryable<Reservation> existingReservations =
      from existingReservation in _context.Reservations
      where
        existingReservation.ReservationTime == reservation.ReservationTime
        && existingReservation.ProviderId == reservation.ProviderID
        && (
          existingReservation.Expiration > DateTime.Now
          || existingReservation.Confirmed == true
        )
      select existingReservation;

    if (existingReservations.Count() > 0)
    {
      throw new AppointmentAlreadyBookedException(
        reservation.ReservationTime,
        reservation.ClientID,
        reservation.ProviderID
      );
    }

    Reservation newReservation = reservation.FromReservationCreateDTO();

    // This does not check if the users exist for the reservation. This is typically only a problem
    // with the in-memory database; as a result I have elected to not include code to handle that
    // problem here.
    //
    // In general, we should not be able to create reservations without users, provided that we have
    // the foreign key constraints set up. For example, this request should be rejected by a
    // PostGres database, as it would violate a foreign key constraint on insertion.
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
  public User? GetUser(Guid userID)
  {
    if (_context.Users == null)
    {
      return null;
    }

    User? user = _context.Users.Find(userID);

    return user;
  }
  
  /// <inheritdoc/>
  public IEnumerable<Availability> GetAvailabilitiesByProvider(
    Guid providerID,
    DateTime? start = null,
    DateTime? end = null
  )
  {
    if (_context.Availabilities == null) {
      return new List<Availability>();
    }

    IQueryable<Availability> availabilities =
      from availability in _context.Availabilities
      where availability.ProviderId == providerID
      select availability;

    if (start != null)
    {
      availabilities =
        from availability in availabilities
        where availability.Start <= start
        select availability;
    }

    if (end != null)
    {
      availabilities =
        from availability in availabilities
        where availability.End >= end
        select availability;
    }

    return availabilities.ToList();
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
