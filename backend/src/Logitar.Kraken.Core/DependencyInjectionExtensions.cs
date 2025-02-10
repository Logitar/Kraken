using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Dictionaries;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Validators;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.Core.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenCore(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcing()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddManagers()
      .AddSingleton<IAddressHelper, AddressHelper>()
      .AddTransient<IFieldValueValidatorFactory, FieldValueValidatorFactory>();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IContentManager, ContentManager>()
      .AddTransient<IContentTypeManager, ContentTypeManager>()
      .AddTransient<IDictionaryManager, DictionaryManager>()
      .AddTransient<IFieldTypeManager, FieldTypeManager>()
      .AddTransient<ILanguageManager, LanguageManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<IRoleManager, RoleManager>()
      .AddTransient<ITemplateManager, TemplateManager>()
      .AddTransient<IUserManager, UserManager>();
  }
}
