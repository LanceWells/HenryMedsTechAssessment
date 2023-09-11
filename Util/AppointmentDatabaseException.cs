using System.Text.Json;

namespace HenryMeds.Util;

public class AppointmentDatabaseException : Exception
{
  public AppointmentDatabaseException() {}
}

public class AppointmentAlreadyBookedException : AppointmentDatabaseException
{
  public DateTime TimeBooked { get; }

  public Guid ClientID { get; }

  public Guid ProviderID { get; }

  public AppointmentAlreadyBookedException(
    DateTime timeBooked,
    Guid clientID,
    Guid providerID
  ) {
    TimeBooked = timeBooked;
    ClientID = clientID;
    ProviderID = providerID;
  }

  public override string ToString()
  {
    return JsonSerializer.Serialize(this);
  }
}
