namespace HenryMeds.Models;

/// <summary>
/// An interface used to connect with a given implementation of an appointments repository.
/// </summary>
public interface IAppointmentsRepository
{
  /// <summary>
  /// Creates a new user in the database.
  /// </summary>
  /// <param name="user">The user to create.</param>
  /// <returns>
  /// The user that was created. <c>null</c> if there were any errors in creation.
  /// </returns>
  User? CreateUser(UserCreateDTO user);

  /// <summary>
  /// Gets a user from the database.
  /// </summary>
  /// <param name="userID">The ID for the user to find.</param>
  /// <returns>The user with the provided ID, if one was found.</returns>
  User? GetUser(Guid userID);

  /// <summary>
  /// Creates a new availability in the database.
  /// </summary>
  /// <param name="availability">The availability to create.</param>
  /// <param name="providerID">The ID for the associated provider.</param>
  /// <returns>
  /// The availability that was created. <c>null</c> if there were any errors in creation.
  /// </returns>
  Availability? CreateAvailability(AvailabilityCreateDTO availability, Guid providerID);

  /// <summary>
  /// Gets a list of availabilities for the given provider. This need to be processed into something
  /// that can read as a set of 15-minute increments. That perhaps should be set in the frontend,
  /// and be validated by the backend.
  /// </summary>
  /// <param name="providerID">The ID for the provider to get availabilities for.</param>
  /// <param name="start">A filter for the time at which an appointment can start (inclusive)</param>
  /// <param name="end">A filter for the time at which an appointment can end (inclusive)</param>
  /// <returns>The availabilities for the provider. <c>null</c> if there were any errors.</returns>
  IEnumerable<Availability> GetAvailabilitiesByProvider(
    Guid providerID,
    DateTime? start = null,
    DateTime? end = null
  );

  /// <summary>
  /// Creates a new reservation in the database. Note that the reservation time must be a valid time
  /// within a provider's set of availabilities.
  /// </summary>
  /// <param name="reservation">The reservation to create.</param>
  /// <returns>
  /// The reservation that was created. <c>null</c> if there were any errors in creation.
  /// </returns>
  Reservation? CreateReservation(ReservationCreateDTO reservation);

  /// <summary>
  /// Updates the given reservation with the possible mutable information for that reservation. This
  /// is primarily used to confirm an appointment.
  /// </summary>
  /// <param name="reservation">A reference to the reservation to update.</param>
  /// <param name="reservationDTO">The information on the reservation to update.</param>
  /// <returns>
  /// The reservation that was updated. <c>null</c> if there were any errors in the update.
  /// </returns>
  Reservation? UpdateReservation(
    Reservation reservation,
    ReservationUpdateDTO reservationDTO
  );

  /// <summary>
  /// Gets a given reservation from the database.
  /// </summary>
  /// <param name="reservationID">The ID for the reservation to fetch.</param>
  /// <returns>
  /// The reservation to be fetched. <c>null</c> if it was not found.
  /// </returns>
  Reservation? GetReservation(Guid reservationID);
}
