using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Contracts.Users;
using MediatR;

namespace Logitar.Kraken.Core.Users.Queries;

public record ReadUserQuery(Guid? Id, string? UniqueName, CustomIdentifierModel? Identifier) : Activity, IRequest<UserModel?>;

internal class ReadUserQueryHandler : IRequestHandler<ReadUserQuery, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;

  public ReadUserQueryHandler(IApplicationContext applicationContext, IUserQuerier userQuerier)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
  }

  public async Task<UserModel?> Handle(ReadUserQuery query, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    Dictionary<Guid, UserModel> users = new(capacity: 3);

    if (query.Id.HasValue)
    {
      UserModel? user = await _userQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      UserModel? user = await _userQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
      else if (userSettings.RequireUniqueEmail)
      {
        EmailModel email = new(query.UniqueName);
        IReadOnlyCollection<UserModel> usersByEmail = await _userQuerier.ReadAsync(email, cancellationToken);
        if (usersByEmail.Count == 1)
        {
          user = usersByEmail.Single();
          users[user.Id] = user;
        }
      }
    }

    if (query.Identifier != null)
    {
      UserModel? user = await _userQuerier.ReadAsync(query.Identifier, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw TooManyResultsException<UserModel>.ExpectedSingle(users.Count);
    }

    return users.Values.SingleOrDefault();
  }
}
