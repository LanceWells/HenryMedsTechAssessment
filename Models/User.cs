namespace HenryMeds.Models;

/// <summary>
/// A user that might either be a client or a provider.
/// </summary>
public class User
{
  /// <summary>
  /// The ID for the given user.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The type of user. Ideally this should determine which permissions and interactions this user
  /// is enabled to have.
  /// </summary>
  public required UserType UserType { get; set; }
}

/// <summary>
/// A DTO used to create a new instance of a <see cref="User"/>.
/// </summary>
public class UserCreateDTO
{
  /// <summary>
  /// Updates <see cref="User.UserType"/>.
  /// </summary>
  public UserType UserType { get; set; }

  /// <summary>
  /// Creates a new instance of a <see cref="User"/>.
  /// </summary>
  /// <returns>A new instance of a <see cref="User"/>.</returns>
  public User FromUserCreateDTO()
  {
    User u = new()
    {
        UserType = UserType
    };
    return u;
  }
}
