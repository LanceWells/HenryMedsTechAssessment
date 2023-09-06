namespace HenryMeds.Models;

public interface IAppointmentsRepository
{
  User? CreateUser(UserCreateDTO user);

  Availability? CreateAvailability(AvailabilityCreateDTO availability);

  IEnumerable<Availability> GetAvailabilitiesByProvider(Guid providerID);

  Reservation? CreateReservation(ReservationCreateDTO reservation);

  Reservation? UpdateReservation(
    Reservation reservation,
    ReservationUpdateDTO reservationDTO
  );

  Reservation? GetReservation(Guid reservationID);
}
