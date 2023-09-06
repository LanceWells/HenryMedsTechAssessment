namespace HenryMeds.Models;

/// <summary>
/// The type of user. This should determine which permissions and interactions a user is able to
/// perform.
/// </summary>
public enum UserType
{
  /// <summary>
  /// A provider. Providers are able to set availabilities for clients to book.
  /// </summary>
  Provider,
  
  /// <summary>
  /// A client. Clients are able to book reservations within a provider's list of availabilities.
  /// </summary>
  Client,
}
