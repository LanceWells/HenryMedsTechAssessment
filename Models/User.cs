namespace HenryMeds.Models;

public class User
{
  public Guid Id { get; set; }

  public required UserType UserType { get; set; }
}

public class UserCreateDTO
{
  public UserType UserType { get; set; }

  public User FromUserCreateDTO()
  {
    User u = new()
    {
        UserType = UserType
    };
    return u;
  }
}
