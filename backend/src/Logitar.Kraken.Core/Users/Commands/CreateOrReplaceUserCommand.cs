using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Users.Validators;
using MediatR;
using TimeZone = Logitar.Kraken.Core.Localization.TimeZone;

namespace Logitar.Kraken.Core.Users.Commands;

public record CreateOrReplaceUserResult(UserModel? User = null, bool Created = false);

public record CreateOrReplaceUserCommand(Guid? Id, CreateOrReplaceUserPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceUserResult>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    CreateOrReplaceUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class CreateOrReplaceUserCommandHandler : IRequestHandler<CreateOrReplaceUserCommand, CreateOrReplaceUserResult>
{
  private readonly IAddressHelper _addressHelper;
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleManager _roleManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateOrReplaceUserCommandHandler(
    IAddressHelper addressHelper,
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    IRoleManager roleManager,
    IUserManager userManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _addressHelper = addressHelper;
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _roleManager = roleManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<CreateOrReplaceUserResult> Handle(CreateOrReplaceUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    CreateOrReplaceUserPayload payload = command.Payload;
    new CreateOrReplaceUserValidator(userSettings, _addressHelper).ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    UserId userId = UserId.NewId(realmId);
    User? user = null;
    if (command.Id.HasValue)
    {
      userId = new(command.Id.Value, realmId);
      user = await _userRepository.LoadAsync(userId, cancellationToken);
    }

    ActorId? actorId = _applicationContext.ActorId;
    UniqueName uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    Password? password = payload.Password == null ? null : _passwordManager.ValidateAndHash(payload.Password);

    bool created = false;
    if (user == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceUserResult();
      }

      user = new(uniqueName, password, actorId, userId);
      created = true;
    }
    else if (password != null)
    {
      user.SetPassword(password, actorId);
    }

    User reference = (command.Version.HasValue
      ? await _userRepository.LoadAsync(user.Id, command.Version.Value, cancellationToken)
      : null) ?? user;

    ReplaceAuthenticationInformation(userSettings, payload, user, reference, actorId);
    ReplaceContactInformation(payload, user, reference, actorId);
    ReplacePersonalInformation(payload, user, reference);

    user.SetCustomAttributes(payload.CustomAttributes, reference);

    await SetRolesAsync(payload, reference, user, actorId, cancellationToken);

    user.Update(actorId);
    await _userManager.SaveAsync(user, cancellationToken);

    UserModel model = await _userQuerier.ReadAsync(user, cancellationToken);
    return new CreateOrReplaceUserResult(model, created);
  }

  private static void ReplaceAuthenticationInformation(IUserSettings userSettings, CreateOrReplaceUserPayload payload, User user, User reference, ActorId? actorId)
  {
    UniqueName uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    if (reference.UniqueName != uniqueName)
    {
      user.SetUniqueName(uniqueName, actorId);
    }

    if (reference.IsDisabled != payload.IsDisabled)
    {
      if (payload.IsDisabled)
      {
        user.Disable(actorId);
      }
      else
      {
        user.Enable(actorId);
      }
    }
  }

  private void ReplaceContactInformation(CreateOrReplaceUserPayload payload, User user, User reference, ActorId? actorId)
  {
    Address? address = payload.Address == null ? null : new(_addressHelper, payload.Address, payload.Address.IsVerified);
    if (reference.Address != address)
    {
      user.SetAddress(address, actorId);
    }

    Email? email = payload.Email == null ? null : new(payload.Email, payload.Email.IsVerified);
    if (reference.Email != email)
    {
      user.SetEmail(email, actorId);
    }

    Phone? phone = payload.Phone == null ? null : new(payload.Phone, payload.Phone.IsVerified);
    if (reference.Phone != phone)
    {
      user.SetPhone(phone, actorId);
    }
  }

  private static void ReplacePersonalInformation(CreateOrReplaceUserPayload payload, User user, User reference)
  {
    PersonName? firstName = PersonName.TryCreate(payload.FirstName);
    if (reference.FirstName != firstName)
    {
      user.FirstName = firstName;
    }
    PersonName? middleName = PersonName.TryCreate(payload.MiddleName);
    if (reference.MiddleName != middleName)
    {
      user.MiddleName = middleName;
    }
    PersonName? lastName = PersonName.TryCreate(payload.LastName);
    if (reference.LastName != lastName)
    {
      user.LastName = lastName;
    }
    PersonName? nickname = PersonName.TryCreate(payload.Nickname);
    if (reference.Nickname != nickname)
    {
      user.Nickname = nickname;
    }

    if (reference.Birthdate != payload.Birthdate)
    {
      user.Birthdate = payload.Birthdate;
    }
    Gender? gender = Gender.TryCreate(payload.Gender);
    if (reference.Gender != gender)
    {
      user.Gender = gender;
    }
    Locale? locale = Locale.TryCreate(payload.Locale);
    if (reference.Locale != locale)
    {
      user.Locale = locale;
    }
    TimeZone? timeZone = TimeZone.TryCreate(payload.TimeZone);
    if (reference.TimeZone != timeZone)
    {
      user.TimeZone = timeZone;
    }

    Url? picture = Url.TryCreate(payload.Picture);
    if (reference.Picture != picture)
    {
      user.Picture = picture;
    }
    Url? profile = Url.TryCreate(payload.Profile);
    if (reference.Profile != profile)
    {
      user.Profile = profile;
    }
    Url? website = Url.TryCreate(payload.Website);
    if (reference.Website != website)
    {
      user.Website = website;
    }
  }

  private async Task SetRolesAsync(CreateOrReplaceUserPayload payload, User reference, User user, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<RoleId, Role> roles = (await _roleManager.FindAsync(payload.Roles, nameof(payload.Roles), cancellationToken))
      .ToDictionary(x => x.Value.Id, x => x.Value);

    foreach (RoleId roleId in reference.Roles)
    {
      if (!roles.ContainsKey(roleId))
      {
        user.RemoveRole(roleId, actorId);
      }
    }

    foreach (Role role in roles.Values)
    {
      if (!reference.HasRole(role))
      {
        user.AddRole(role, actorId);
      }
    }
  }
}
