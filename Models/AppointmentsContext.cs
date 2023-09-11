using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HenryMeds.Models;

/// <summary>
/// The context object that maintains the connection to the Appointments database.
/// </summary>
public class AppointmentsInMemoryContext : DbContext
{
  /// <summary>
  /// The name used to refer to the apointment database.
  /// </summary>
  public static string AppointmentDbName = "Appointments";

  /// <summary>
  /// A reference to the table of <see cref="Availability"/> objects in this database.
  /// </summary>
  public DbSet<Availability>? Availabilities { get; set; }

  /// <summary>
  /// A reference to the table of <see cref="Reservation"/> objects in this database.
  /// </summary>
  public DbSet<Reservation>? Reservations { get; set; }

  /// <summary>
  /// A reference to the table of <see cref="User"/> objects in this database.
  /// </summary>
  public DbSet<User>? Users { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseInMemoryDatabase(AppointmentDbName);
    base.OnConfiguring(optionsBuilder);
  }
}
