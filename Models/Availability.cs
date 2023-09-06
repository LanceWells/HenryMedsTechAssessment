namespace HenryMeds.Models;

public class Availability
{
  public Guid Id { get; set; }

  public required DateTime Start { get; set; }

  public required DateTime End { get; set; }

  public User Provider { get; set; } = null!;

  public required Guid ProviderId { get; set; }
}

public class AvailabilityCreateDTO
{
  public DateTime Start { get; set; }

  public DateTime End { get; set; }

  public Guid ProviderId { get; set; }

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
