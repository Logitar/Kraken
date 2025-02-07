using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Logitar.Kraken.EntityFrameworkCore.Relational;
using Logitar.Kraken.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Kraken.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarKrakenEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_Kraken");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      connectionString = configuration.GetConnectionString("SqlServer");
    }
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The connection string for the database provider '{DatabaseProvider.EntityFrameworkCoreSqlServer}' could not be found.", nameof(configuration));
    }
    return services.AddLogitarKrakenEntityFrameworkCoreSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarKrakenEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<KrakenContext>(options => options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("Logitar.Kraken.EntityFrameworkCore.SqlServer")))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString);
  }
}
