namespace HenryMeds.Models;

/// <summary>
/// A reservation used to refer to when clients and providers meet.
/// </summary>
public class Reservation
{
  /// <summary>
  /// The ID for this reservation.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The time at which the reservation is desired. Note that this is considered to be a 15-minute
  /// reservation.
  /// </summary>
  public required DateTime ReservationTime { get; set; }

  /// <summary>
  /// The time at which the reservation will expire. By default this is 30 minutes after its
  /// creation.
  /// </summary>
  public required DateTime Expiration { get; set; }

  /// <summary>
  /// Whether or not the given reservation has been confirmed by the client.
  /// </summary>
  public required bool Confirmed { get; set; }

  /// <summary>
  /// A reference to the client that has requested this reservation.
  /// </summary>
  public User Client { get; set; } = null!;

  /// <summary>
  /// A reference to the provider that has been requested for this reservation.
  /// </summary>
  public User Provider { get; set; } = null!;

  /// <summary>
  /// The ID for the client. This is a foreign key for <see cref="Client"/> .
  /// </summary>
  public required Guid ClientId { get; set; }

  /// <summary>
  /// The ID for the provider. This is a foreign key for <see cref="Provider"/> .
  /// </summary>
  public required Guid ProviderId { get; set; }
}

/// <summary>
/// A DTO used to create the <see cref="Reservation"/> .
/// </summary>
public class ReservationCreateDTO
{
  /// <summary>
  /// Updates <see cref="Reservation.ReservationTime"/> .
  /// </summary>
  public DateTime ReservationTime { get; set; }

  /// <summary>
  /// Updates <see cref="Reservation.ClientID"/> .
  /// </summary>
  public Guid ClientID { get; set; }

  /// <summary>
  /// Updates <see cref="Reservation.ProviderID"/> .
  /// </summary>
  public Guid ProviderID { get; set; }

  /// <summary>
  /// Creates a new instance of a <see cref="Reservation"/> from this object.
  /// </summary>
  /// <returns>A new instance of an <see cref="Reservation"/> from this object.</returns>
  public Reservation FromReservationCreateDTO()
  {
    Reservation r = new()
    {
      ClientId = ClientID,
      Expiration = DateTime.Now + TimeSpan.FromMinutes(30),
      ProviderId = ProviderID,
      ReservationTime = ReservationTime,
      Confirmed = false,
    };

    return r;
  }
}

/// <summary>
/// A DTO used to update an existing <see cref="Reservation"/>. The fields in this class are
/// implied to be mutable.
/// </summary>
public class ReservationUpdateDTO
{
  /// <summary>
  /// Updates <see cref="Reservation.Confirmed"/> .
  /// </summary>
  public bool? Confirmed { get; set; }

  /// <summary>
  /// Creates a new instance of a <see cref="Reservation"/> from this object.
  /// </summary>
  /// <param name="reservation">The reservation to update with the contents of this object.</param>
  /// <returns>A new instance of a <see cref="Reservation"/> from this object.</returns>
  public Reservation WithReservation(Reservation reservation)
  {
    Reservation r = new()
    {
      ClientId = reservation.ClientId,
      Confirmed = Confirmed ?? reservation.Confirmed,
      Expiration = reservation.Expiration,
      ProviderId = reservation.ProviderId,
      ReservationTime = reservation.ReservationTime,
    };

    return r;
  }
}
