using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public class KrakenContext : DbContext
{
  public KrakenContext(DbContextOptions<KrakenContext> options) : base(options)
  {
  }

  #region Kraken
  public DbSet<ConfigurationEntity> Configurations => Set<ConfigurationEntity>();
  public DbSet<RealmEntity> Realms => Set<RealmEntity>();
  #endregion

  #region Localization
  public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();
  #endregion

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
