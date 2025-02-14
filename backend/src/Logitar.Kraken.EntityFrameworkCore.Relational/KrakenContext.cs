using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public class KrakenContext : DbContext
{
  public KrakenContext(DbContextOptions<KrakenContext> options) : base(options)
  {
  }

  #region Identity
  public DbSet<ApiKeyEntity> ApiKeys => Set<ApiKeyEntity>();
  public DbSet<ApiKeyRoleEntity> ApiKeyRoles => Set<ApiKeyRoleEntity>();
  public DbSet<OneTimePasswordEntity> OneTimePasswords => Set<OneTimePasswordEntity>();
  public DbSet<RoleEntity> Roles => Set<RoleEntity>();
  public DbSet<SessionEntity> Sessions => Set<SessionEntity>();
  public DbSet<BlacklistedTokenEntity> TokenBlacklist => Set<BlacklistedTokenEntity>();
  public DbSet<UserEntity> Users => Set<UserEntity>();
  public DbSet<UserIdentifierEntity> UserIdentifiers => Set<UserIdentifierEntity>();
  public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
  #endregion

  #region Kraken
  public DbSet<ConfigurationEntity> Configurations => Set<ConfigurationEntity>();
  public DbSet<CustomAttributeEntity> CustomAttributes => Set<CustomAttributeEntity>();
  public DbSet<RealmEntity> Realms => Set<RealmEntity>();
  #endregion

  #region Localization
  public DbSet<DictionaryEntity> Dictionaries => Set<DictionaryEntity>();
  public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();
  #endregion

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
