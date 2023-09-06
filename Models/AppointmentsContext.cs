using Microsoft.EntityFrameworkCore;

namespace HenryMeds.Models;

public class AppointmentsContext : DbContext
{
  public static string AppointmentDbName = "Appointments";

  public DbSet<Availability>? Availabilities { get; set; }

  public DbSet<Reservation>? Reservations { get; set; }

  public DbSet<User>? Users { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseInMemoryDatabase(AppointmentDbName);
    base.OnConfiguring(optionsBuilder);
  }
}
