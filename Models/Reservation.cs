using System.ComponentModel;

namespace HenryMeds.Models;

public class Reservation
{
  public Guid Id { get; set; }

  public required DateTime ReservationTime { get; set; }

  public required DateTime Expiration { get; set; }

  public required bool Confirmed { get; set; }

  public User Client { get; set; } = null!;

  public User Provider { get; set; } = null!;

  public required Guid ClientId { get; set; }

  public required Guid ProviderId { get; set; }
}

public class ReservationCreateDTO
{
  public DateTime ReservationTime { get; set; }

  public Guid ClientID { get; set; }

  public Guid ProviderID { get; set; }

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

public class ReservationUpdateDTO
{
  public bool? Confirmed { get; set; }

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
