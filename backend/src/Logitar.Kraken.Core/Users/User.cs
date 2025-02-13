using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.Core.Users.Events;
using TimeZone = Logitar.Kraken.Core.Localization.TimeZone;

namespace Logitar.Kraken.Core.Users;

public class User : AggregateRoot, ICustomizable
{
  private UserUpdated _updatedEvent = new();

  public new UserId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The user has not been initialized yet.");

  private Password? _password = null;
  public bool HasPassword => _password != null;

  public bool IsDisabled { get; private set; }

  public Address? Address { get; private set; }
  public Email? Email { get; private set; }
  public Phone? Phone { get; private set; }
  public bool IsConfirmed => (Address != null && Address.IsVerified) || (Email != null && Email.IsVerified) || (Phone != null && Phone.IsVerified);

  private PersonName? _firstName = null;
  public PersonName? FirstName
  {
    get => _firstName;
    set
    {
      if (_firstName != value)
      {
        _firstName = value;
        _updatedEvent.FirstName = new Change<PersonName>(value);

        FullName = PersonName.BuildFullName(_firstName, _middleName, _lastName);
        _updatedEvent.FullName = new Change<string>(FullName);
      }
    }
  }
  private PersonName? _middleName = null;
  public PersonName? MiddleName
  {
    get => _middleName;
    set
    {
      if (_middleName != value)
      {
        _middleName = value;
        _updatedEvent.MiddleName = new Change<PersonName>(value);

        FullName = PersonName.BuildFullName(_firstName, _middleName, _lastName);
        _updatedEvent.FullName = new Change<string>(FullName);
      }
    }
  }
  private PersonName? _lastName = null;
  public PersonName? LastName
  {
    get => _lastName;
    set
    {
      if (_lastName != value)
      {
        _lastName = value;
        _updatedEvent.LastName = new Change<PersonName>(value);

        FullName = PersonName.BuildFullName(_firstName, _middleName, _lastName);
        _updatedEvent.FullName = new Change<string>(FullName);
      }
    }
  }
  public string? FullName { get; private set; }
  private PersonName? _nickname = null;
  public PersonName? Nickname
  {
    get => _nickname;
    set
    {
      if (_nickname != value)
      {
        _nickname = value;
        _updatedEvent.Nickname = new Change<PersonName>(value);
      }
    }
  }

  private DateTime? _birthdate = null;
  public DateTime? Birthdate // TODO(fpion): unit tests
  {
    get => _birthdate;
    set
    {
      if (_birthdate != value)
      {
        if (value.HasValue && value.Value.AsUniversalTime() >= DateTime.UtcNow)
        {
          throw new ArgumentOutOfRangeException(nameof(Birthdate), "The value must be a date and time set in the past.");
        }

        _birthdate = value;
        _updatedEvent.Birthdate = new Change<DateTime?>(value);
      }
    }
  }
  private Gender? _gender = null;
  public Gender? Gender
  {
    get => _gender;
    set
    {
      if (_gender != value)
      {
        _gender = value;
        _updatedEvent.Gender = new Change<Gender>(value);
      }
    }
  }
  private Locale? _locale = null;
  public Locale? Locale
  {
    get => _locale;
    set
    {
      if (_locale != value)
      {
        _locale = value;
        _updatedEvent.Locale = new Change<Locale>(value);
      }
    }
  }
  private TimeZone? _timeZone = null;
  public TimeZone? TimeZone
  {
    get => _timeZone;
    set
    {
      if (_timeZone != value)
      {
        _timeZone = value;
        _updatedEvent.TimeZone = new Change<TimeZone>(value);
      }
    }
  }

  private Url? _picture = null;
  public Url? Picture
  {
    get => _picture;
    set
    {
      if (_picture != value)
      {
        _picture = value;
        _updatedEvent.Picture = new Change<Url>(value);
      }
    }
  }
  private Url? _profile = null;
  public Url? Profile
  {
    get => _profile;
    set
    {
      if (_profile != value)
      {
        _profile = value;
        _updatedEvent.Profile = new Change<Url>(value);
      }
    }
  }
  private Url? _website = null;
  public Url? Website
  {
    get => _website;
    set
    {
      if (_website != value)
      {
        _website = value;
        _updatedEvent.Website = new Change<Url>(value);
      }
    }
  }

  public DateTime? AuthenticatedOn { get; private set; }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  private readonly Dictionary<Identifier, CustomIdentifier> _customIdentifiers = [];
  public IReadOnlyDictionary<Identifier, CustomIdentifier> CustomIdentifiers => _customIdentifiers.AsReadOnly();

  private readonly HashSet<RoleId> _roles = [];
  public IReadOnlyCollection<RoleId> Roles => _roles.ToList().AsReadOnly();

  public User() : base()
  {
  }

  public User(UniqueName uniqueName, Password? password = null, ActorId? actorId = null, UserId? userId = null)
    : base(userId?.StreamId)
  {
    Raise(new UserCreated(uniqueName, password), actorId);
  }
  protected virtual void Handle(UserCreated @event)
  {
    _uniqueName = @event.UniqueName;

    _password = @event.Password;
  }

  public void AddRole(Role role, ActorId? actorId = null) // TODO(fpion): unit tests
  {
    // TODO(fpion): ensure is in same realm

    if (!HasRole(role))
    {
      Raise(new UserRoleAdded(role.Id), actorId);
    }
  }
  protected virtual void Handle(UserRoleAdded @event)
  {
    _roles.Add(@event.RoleId);
  }

  public void Authenticate(string password, ActorId? actorId = null)
  {
    if (IsDisabled)
    {
      throw new UserIsDisabledException(this);
    }
    else if (_password == null)
    {
      throw new UserHasNoPasswordException(this);
    }
    else if (!_password.IsMatch(password))
    {
      throw new IncorrectUserPasswordException(this, password);
    }

    actorId ??= new(Id.Value);
    Raise(new UserAuthenticated(), actorId.Value);
  }
  protected virtual void Handle(UserAuthenticated @event)
  {
    AuthenticatedOn = @event.OccurredOn;
  }

  public void ChangePassword(string currentPassword, Password newPassword, ActorId? actorId = null) // TODO(fpion): unit tests
  {
    if (IsDisabled)
    {
      throw new UserIsDisabledException(this);
    }
    else if (_password == null)
    {
      throw new UserHasNoPasswordException(this);
    }
    else if (!_password.IsMatch(currentPassword))
    {
      throw new IncorrectUserPasswordException(this, currentPassword);
    }

    actorId ??= new(Id.Value);
    Raise(new UserPasswordChanged(newPassword), actorId.Value);
  }
  protected virtual void Handle(UserPasswordChanged @event)
  {
    _password = @event.Password;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new UserDeleted(), actorId);
    }
  }

  public void Disable(ActorId? actorId = null)
  {
    if (!IsDisabled)
    {
      Raise(new UserDisabled(), actorId);
    }
  }
  protected virtual void Handle(UserDisabled _)
  {
    IsDisabled = true;
  }

  public void Enable(ActorId? actorId = null)
  {
    if (IsDisabled)
    {
      Raise(new UserEnabled(), actorId);
    }
  }
  protected virtual void Handle(UserEnabled _)
  {
    IsDisabled = false;
  }

  public bool HasRole(Role role) => HasRole(role.Id); // TODO(fpion): unit tests
  public bool HasRole(RoleId roleId) => _roles.Contains(roleId); // TODO(fpion): unit tests

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updatedEvent.CustomAttributes[key] = null;
    }
  }

  public void RemoveCustomIdentifier(Identifier key, ActorId? actorId = null) // TODO(fpion): unit tests
  {
    if (_customIdentifiers.ContainsKey(key))
    {
      Raise(new UserIdentifierRemoved(key), actorId);
    }
  }
  protected virtual void Handle(UserIdentifierRemoved @event)
  {
    _customIdentifiers.Remove(@event.Key);
  }

  public void RemovePassword(ActorId? actorId = null) // TODO(fpion): unit tests
  {
    if (_password != null)
    {
      Raise(new UserPasswordRemoved(), actorId);
    }
  }
  protected virtual void Handle(UserPasswordRemoved @event)
  {
    _password = null;
  }

  public void RemoveRole(Role role, ActorId? actorId = null) // TODO(fpion): unit tests
  {
    if (HasRole(role))
    {
      Raise(new UserRoleRemoved(role.Id), actorId);
    }
  }
  public void RemoveRole(RoleId roleId, ActorId? actorId = null)
  {
    if (HasRole(roleId))
    {
      Raise(new UserRoleRemoved(roleId), actorId);
    }
  }
  protected virtual void Handle(UserRoleRemoved @event)
  {
    _roles.Remove(@event.RoleId);
  }

  public void ResetPassword(Password password, ActorId? actorId = null)
  {
    if (IsDisabled)
    {
      throw new UserIsDisabledException(this);
    }

    actorId ??= new(Id.Value);
    Raise(new UserPasswordReset(password), actorId.Value);
  }
  protected virtual void Handle(UserPasswordReset @event)
  {
    _password = @event.Password;
  }

  public void SetAddress(Address? address, ActorId? actorId = null)
  {
    if (Address != address)
    {
      Raise(new UserAddressChanged(address), actorId);
    }
  }
  protected virtual void Handle(UserAddressChanged @event)
  {
    Address = @event.Address;
  }

  public void SetCustomAttribute(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveCustomAttribute(key);
    }
    else
    {
      value = value.Trim();
      if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
      {
        _customAttributes[key] = value;
        _updatedEvent.CustomAttributes[key] = value;
      }
    }
  }

  public void SetCustomIdentifier(Identifier key, CustomIdentifier value, ActorId? actorId = null) // TODO(fpion): unit tests
  {
    if (!_customIdentifiers.TryGetValue(key, out CustomIdentifier? existingValue) || existingValue != value)
    {
      Raise(new UserIdentifierChanged(key, value), actorId);
    }
  }
  protected virtual void Handle(UserIdentifierChanged @event)
  {
    _customIdentifiers[@event.Key] = @event.Value;
  }

  public void SetEmail(Email? email, ActorId? actorId = null)
  {
    if (Email != email)
    {
      Raise(new UserEmailChanged(email), actorId);
    }
  }
  protected virtual void Handle(UserEmailChanged @event)
  {
    Email = @event.Email;
  }

  public void SetPassword(Password password, ActorId? actorId = null)
  {
    Raise(new UserPasswordUpdated(password), actorId);
  }
  protected virtual void Handle(UserPasswordUpdated @event)
  {
    _password = @event.Password;
  }

  public void SetPhone(Phone? phone, ActorId? actorId = null)
  {
    if (Phone != phone)
    {
      Raise(new UserPhoneChanged(phone), actorId);
    }
  }
  protected virtual void Handle(UserPhoneChanged @event)
  {
    Phone = @event.Phone;
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new UserUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(UserUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public Session SignIn(Password? secret = null, ActorId? actorId = null, Guid? sessionId = null)
  {
    return SignIn(password: null, secret, actorId, sessionId);
  }
  public Session SignIn(string? password, Password? secret = null, ActorId? actorId = null, Guid? sessionId = null) // TODO(fpion): unit tests
  {
    if (IsDisabled)
    {
      throw new UserIsDisabledException(this);
    }
    else if (password != null)
    {
      if (_password == null)
      {
        throw new UserHasNoPasswordException(this);
      }
      else if (!_password.IsMatch(password))
      {
        throw new IncorrectUserPasswordException(this, password);
      }
    }

    actorId ??= new(Id.Value);
    SessionId id = sessionId.HasValue ? new SessionId(sessionId.Value, RealmId) : SessionId.NewId(Id.RealmId);
    Session session = new(this, secret, actorId, id);
    Raise(new UserSignedIn(session.CreatedOn), actorId.Value);

    return session;
  }
  protected virtual void Handle(UserSignedIn @event)
  {
    AuthenticatedOn = @event.OccurredOn;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Handle(UserUpdated @event)
  {
    if (@event.FirstName != null)
    {
      _firstName = @event.FirstName.Value;
    }
    if (@event.MiddleName != null)
    {
      _middleName = @event.MiddleName.Value;
    }
    if (@event.LastName != null)
    {
      _lastName = @event.LastName.Value;
    }
    if (@event.FullName != null)
    {
      FullName = @event.FullName.Value;
    }
    if (@event.Nickname != null)
    {
      _nickname = @event.Nickname.Value;
    }

    if (@event.Birthdate != null)
    {
      _birthdate = @event.Birthdate.Value;
    }
    if (@event.Gender != null)
    {
      _gender = @event.Gender.Value;
    }
    if (@event.Locale != null)
    {
      _locale = @event.Locale.Value;
    }
    if (@event.TimeZone != null)
    {
      _timeZone = @event.TimeZone.Value;
    }

    if (@event.Picture != null)
    {
      _picture = @event.Picture.Value;
    }
    if (@event.Profile != null)
    {
      _profile = @event.Profile.Value;
    }
    if (@event.Website != null)
    {
      _website = @event.Website.Value;
    }

    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        _customAttributes.Remove(customAttribute.Key);
      }
      else
      {
        _customAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }

  public override string ToString() => $"{FullName ?? UniqueName.Value} | {base.ToString()}";
}
