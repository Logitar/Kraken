using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.Core.Users;

public class User : AggregateRoot, ICustomizable
{
  private readonly UserUpdated _updatedEvent = new();

  public new UserId Id => new(base.Id);

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The user has not been initialized.");

  private Password? _password = null;
  public bool HasPassword => _password != null;

  public bool IsDisabled { get; private set; }

  // TODO(fpion): Address
  // TODO(fpion): Email
  // TODO(fpion): Phone
  // TODO(fpion): IsConfirmed

  private PersonName? _firstName = null;
  public PersonName? FirstName
  {
    get => _firstName;
    set
    {
      if (_firstName != value)
      {
        _firstName = value;
        FullName = PersonName.BuildFullName(_firstName, _middleName, _lastName);

        _updatedEvent.FirstName = new Change<PersonName>(value);
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
        FullName = PersonName.BuildFullName(_firstName, _middleName, _lastName);

        _updatedEvent.MiddleName = new Change<PersonName>(value);
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
        FullName = PersonName.BuildFullName(_firstName, _middleName, _lastName);

        _updatedEvent.LastName = new Change<PersonName>(value);
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

  // TODO(fpion): Birthdate
  // TODO(fpion): Gender
  // TODO(fpion): Locale
  // TODO(fpion): TimeZone

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

  // TODO(fpion): CustomIdentifiers

  // TODO(fpion): Roles

  public User(UniqueName uniqueName, ActorId? actorId = null, UserId? userId = null) : base(userId?.StreamId)
  {
    Raise(new UserCreated(uniqueName), actorId);
  }
  protected virtual void Handle(UserCreated @event)
  {
    _uniqueName = @event.UniqueName;
  }

  // TODO(fpion): AddRole

  // TODO(fpion): Authenticate

  // TODO(fpion): ChangePassword

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

  // TODO(fpion): HasRole

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updatedEvent.CustomAttributes[key] = null;
    }
  }

  // TODO(fpion): RemoveCustomIdentifier

  // TODO(fpion): RemovePassword

  // TODO(fpion): RemoveRole

  // TODO(fpion): ResetPassword

  // TODO(fpion): SetAddress

  public void SetCustomAttribute(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveCustomAttribute(key);
    }
    value = value.Trim();

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
    {
      _customAttributes[key] = value;
      _updatedEvent.CustomAttributes[key] = value;
    }
  }

  // TODO(fpion): SetCustomIdentifier

  // TODO(fpion): SetEmail

  public void SetPassword(Password password, ActorId? actorId = null)
  {
    Raise(new UserPasswordChanged(password), actorId);
  }
  protected virtual void Handle(UserPasswordChanged @event)
  {
    _password = @event.Password;
  }

  // TODO(fpion): SetPhone

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

  // TODO(fpion): SignIn

  // TODO(fpion): Update

  public override string ToString() => $"{FullName ?? UniqueName.Value} | {base.ToString()}";
}
