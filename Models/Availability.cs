namespace HenryMeds.Models;

/// <summary>
/// An availability for a given provider.
/// </summary>
public class Availability
{
  /// <summary>
  /// The ID for this availability.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The start time for the availability. This denotes when a provider is ready to accept
  /// reservations for this time span.
  /// </summary>
  public required DateTime Start { get; set; }

  /// <summary>
  /// The end time for the availability. This denotes when a provider has stopped accepting
  /// reservations for this time span.
  /// </summary>
  public required DateTime End { get; set; }

  /// <summary>
  /// A reference to the provider that this availability refers to.
  /// </summary>
  public User Provider { get; set; } = null!;

  /// <summary>
  /// The ID for the provider. This is a foreign key for <see cref="Provider"/>.
  /// </summary>
  public required Guid ProviderId { get; set; }
}

/// <summary>
/// A DTO used to create the <see cref="Availability"/>.
/// </summary>
public class AvailabilityCreateDTO
{
  /// <summary>
  /// Updates <see cref="Availability.Start"/>.
  /// </summary>
  public DateTime Start { get; set; }

  /// <summary>
  /// Updates <see cref="Availability.End"/>.
  /// </summary>
  public DateTime End { get; set; }

  /// <summary>
  /// Updates <see cref="Availability.ProviderId"/>.
  /// </summary>
  public Guid ProviderId { get; set; }

  /// <summary>
  /// Creates a new instance of an <see cref="Availability"/> from this object.
  /// </summary>
  /// <returns>A new instance of an <see cref="Availability"/> from this object.</returns>
  public Availability FromAvailabilityCreateDTO()
  {
    Availability a = new()
    {
      Start = Start,
      End = End,
      ProviderId = ProviderId,
    };

    return a;
  }
}
