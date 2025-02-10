using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Messages;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Tokens;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Actors;
using Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;
using Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;
using Logitar.Kraken.EntityFrameworkCore.Relational.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<ITokenBlacklist, TokenBlacklist>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IContentQuerier, ContentQuerier>()
      .AddScoped<IContentTypeQuerier, ContentTypeQuerier>()
      .AddScoped<IDictionaryQuerier, DictionaryQuerier>()
      .AddScoped<IFieldTypeQuerier, FieldTypeQuerier>()
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<IMessageQuerier, MessageQuerier>()
      .AddScoped<IOneTimePasswordQuerier, OneTimePasswordQuerier>()
      .AddScoped<IPublishedContentQuerier, PublishedContentQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<IRoleQuerier, RoleQuerier>()
      .AddScoped<ISenderQuerier, SenderQuerier>()
      .AddScoped<ISessionQuerier, SessionQuerier>()
      .AddScoped<ITemplateQuerier, TemplateQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IApiKeyRepository, ApiKeyRepository>()
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IContentRepository, ContentRepository>()
      .AddScoped<IContentTypeRepository, ContentTypeRepository>()
      .AddScoped<IDictionaryRepository, DictionaryRepository>()
      .AddScoped<IFieldTypeRepository, FieldTypeRepository>()
      .AddScoped<ILanguageRepository, LanguageRepository>()
      .AddScoped<IMessageRepository, MessageRepository>()
      .AddScoped<IOneTimePasswordRepository, OneTimePasswordRepository>()
      .AddScoped<IRealmRepository, RealmRepository>()
      .AddScoped<IRoleRepository, RoleRepository>()
      .AddScoped<ISenderRepository, SenderRepository>()
      .AddScoped<ISessionRepository, SessionRepository>()
      .AddScoped<ITemplateRepository, TemplateRepository>()
      .AddScoped<IUserRepository, UserRepository>();
  }
}
