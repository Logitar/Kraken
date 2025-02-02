using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.Infrastructure;

public class KrakenContext : DbContext
{
  public KrakenContext(DbContextOptions<KrakenContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
