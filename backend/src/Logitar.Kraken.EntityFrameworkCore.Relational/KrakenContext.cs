﻿using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public class KrakenContext : DbContext
{
  public KrakenContext(DbContextOptions<KrakenContext> options) : base(options)
  {
  }

  #region Cms
  public DbSet<ContentEntity> Contents => Set<ContentEntity>();
  public DbSet<ContentLocaleEntity> ContentLocales => Set<ContentLocaleEntity>();
  public DbSet<ContentTypeEntity> ContentTypes => Set<ContentTypeEntity>();
  public DbSet<FieldDefinitionEntity> FieldDefinitions => Set<FieldDefinitionEntity>();
  public DbSet<FieldIndexEntity> FieldIndex => Set<FieldIndexEntity>();
  public DbSet<FieldTypeEntity> FieldTypes => Set<FieldTypeEntity>();
  public DbSet<PublishedContentEntity> PublishedContents => Set<PublishedContentEntity>();
  public DbSet<UniqueIndexEntity> UniqueIndex => Set<UniqueIndexEntity>();
  #endregion

  #region Identity
  public DbSet<ApiKeyEntity> ApiKeys => Set<ApiKeyEntity>();
  public DbSet<ApiKeyRoleEntity> ApiKeyRoles => Set<ApiKeyRoleEntity>();
  public DbSet<BlacklistedTokenEntity> TokenBlacklist => Set<BlacklistedTokenEntity>();
  public DbSet<CustomAttributeEntity> CustomAttributes => Set<CustomAttributeEntity>();
  public DbSet<OneTimePasswordEntity> OneTimePasswords => Set<OneTimePasswordEntity>();
  public DbSet<RoleEntity> Roles => Set<RoleEntity>();
  public DbSet<SessionEntity> Sessions => Set<SessionEntity>();
  public DbSet<UserEntity> Users => Set<UserEntity>();
  public DbSet<UserIdentifierEntity> UserIdentifiers => Set<UserIdentifierEntity>();
  public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
  #endregion

  #region Kraken
  public DbSet<RealmEntity> Realms => Set<RealmEntity>();
  #endregion

  #region Localization
  public DbSet<DictionaryEntity> Dictionaries => Set<DictionaryEntity>();
  public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();
  #endregion

  #region Messaging
  public DbSet<MessageEntity> Messages => Set<MessageEntity>();
  public DbSet<RecipientEntity> Recipients => Set<RecipientEntity>();
  public DbSet<SenderEntity> Senders => Set<SenderEntity>();
  public DbSet<TemplateEntity> Templates => Set<TemplateEntity>();
  #endregion

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
