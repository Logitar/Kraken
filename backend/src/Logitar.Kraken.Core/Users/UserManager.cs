using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.Core.Users;

internal class UserManager : IUserManager
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UserManager(IApplicationContext applicationContext, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> FindAsync(string user, string propertyName, bool includeId, CancellationToken cancellationToken)
  {
    RealmId? realmId = _applicationContext.RealmId;

    if (includeId && Guid.TryParse(user.Trim(), out Guid entityId))
    {
      UserId userId = new(entityId, realmId);
      User? found = await _userRepository.LoadAsync(userId, cancellationToken);
      if (found != null)
      {
        return found;
      }
    }

    UniqueName? uniqueName = null;
    try
    {
      uniqueName = new(new UniqueNameSettings(allowedCharacters: null), user);
    }
    catch (Exception)
    {
    }
    if (uniqueName != null)
    {
      User? found = await _userRepository.LoadAsync(realmId, uniqueName, cancellationToken);
      if (found != null)
      {
        return found;
      }
    }

    if (_applicationContext.UserSettings.RequireUniqueEmail)
    {
      Email? email = null;
      try
      {
        email = new(user);
      }
      catch (Exception)
      {
      }
      if (email != null)
      {
        IReadOnlyCollection<User> found = await _userRepository.LoadAsync(realmId, email, cancellationToken);
        if (found.Count == 1)
        {
          return found.Single();
        }
      }
    }

    if (includeId)
    {
      int index = user.IndexOf(':');
      if (index >= 0)
      {
        Identifier? key = null;
        CustomIdentifier? value = null;
        try
        {
          key = new(user[..index]);
          value = new(user[(index + 1)..]);
        }
        catch (Exception)
        {
        }
        if (key != null && value != null)
        {
          User? found = await _userRepository.LoadAsync(realmId, key, value, cancellationToken);
          if (found != null)
          {
            return found;
          }
        }
      }
    }

    throw new UserNotFoundException(realmId, user, propertyName);
  }

  public async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = false;
    bool hasEmailChanged = false;
    Dictionary<Identifier, CustomIdentifier> identifiers = new(capacity: user.Changes.Count);
    foreach (IEvent change in user.Changes)
    {
      if (change is UserCreated || change is UserUniqueNameChanged)
      {
        hasUniqueNameChanged = true;
      }
      else if (change is UserEmailChanged)
      {
        hasEmailChanged = true;
      }
      else if (change is UserIdentifierChanged identifier)
      {
        identifiers[identifier.Key] = identifier.Value;
      }
    }

    if (hasUniqueNameChanged)
    {
      UserId? conflictId = await _userQuerier.FindIdAsync(user.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(user.Id))
      {
        throw new UniqueNameAlreadyUsedException(user, conflictId.Value);
      }
    }

    if (hasEmailChanged && user.Email != null && _applicationContext.UserSettings.RequireUniqueEmail)
    {
      IReadOnlyCollection<UserId> conflictIds = await _userQuerier.FindIdsAsync(user.Email, cancellationToken);
      foreach (UserId conflictId in conflictIds)
      {
        if (!conflictId.Equals(user.Id))
        {
          throw new EmailAddressAlreadyUsedException(user, conflictId);
        }
      }
    }

    foreach (KeyValuePair<Identifier, CustomIdentifier> identifier in identifiers)
    {
      UserId? conflictId = await _userQuerier.FindIdAsync(identifier.Key, identifier.Value, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(user.Id))
      {
        throw new CustomIdentifierAlreadyUsedException(user, conflictId.Value, identifier.Key, identifier.Value);
      }
    }

    await _userRepository.SaveAsync(user, cancellationToken);
  }
}
