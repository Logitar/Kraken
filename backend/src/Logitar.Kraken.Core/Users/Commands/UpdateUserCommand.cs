using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Roles;
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

public record UpdateUserCommand(Guid Id, UpdateUserPayload Payload) : Activity, IRequest<UserModel?>
{
  public override IActivity Anonymize()
  {
    if (Payload.Password == null)
    {
      return base.Anonymize();
    }

    UpdateUserCommand command = this.DeepClone();
    if (command.Payload.Password != null)
    {
      if (command.Payload.Password.Current != null)
      {
        command.Payload.Password.Current = command.Payload.Password.Current.Mask();
      }
      command.Payload.Password.New = command.Payload.Password.New.Mask();
    }
    return command;
  }
}

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserModel?>
{
  private readonly IAddressHelper _addressHelper;
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleManager _roleManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandHandler(
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

  public async Task<UserModel?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    UpdateUserPayload payload = command.Payload;
    new UpdateUserValidator(userSettings, _addressHelper).ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    UserId userId = new(command.Id, realmId);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    UpdateAuthenticationInformation(userSettings, payload, user, actorId);
    UpdateContactInformation(payload, user, actorId);
    UpdatePersonalInformation(payload, user);

    foreach (CustomAttributeModel customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
    }

    IReadOnlyDictionary<string, Role> roles = await _roleManager.FindAsync(payload.Roles.Select(x => x.Role), cancellationToken);
    foreach (RoleAction action in payload.Roles)
    {
      Role role = roles[action.Role];
      switch (action.Action)
      {
        case CollectionAction.Add:
          user.AddRole(role, actorId);
          break;
        case CollectionAction.Remove:
          user.RemoveRole(role, actorId);
          break;
      }
    }

    user.Update(actorId);
    await _userManager.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }

  private void UpdateAuthenticationInformation(IUserSettings userSettings, UpdateUserPayload payload, User user, ActorId? actorId)
  {
    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(userSettings.UniqueName, payload.UniqueName);
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.Password != null)
    {
      Password newPassword = _passwordManager.Create(userSettings.Password, payload.Password.New);
      if (payload.Password.Current == null)
      {
        user.SetPassword(newPassword, actorId);
      }
      else
      {
        user.ChangePassword(payload.Password.Current, newPassword, new ActorId(user.Id.Value));
      }
    }

    if (payload.IsDisabled.HasValue)
    {
      if (payload.IsDisabled.Value)
      {
        user.Disable(actorId);
      }
      else
      {
        user.Enable(actorId);
      }
    }
  }

  private void UpdateContactInformation(UpdateUserPayload payload, User user, ActorId? actorId)
  {
    if (payload.Address != null)
    {
      Address? address = payload.Address.Value == null ? null : new(_addressHelper, payload.Address.Value, payload.Address.Value.IsVerified);
      user.SetAddress(address, actorId);
    }

    if (payload.Email != null)
    {
      Email? email = payload.Email.Value == null ? null : new(payload.Email.Value, payload.Email.Value.IsVerified);
      user.SetEmail(email, actorId);
    }

    if (payload.Phone != null)
    {
      Phone? phone = payload.Phone.Value == null ? null : new(payload.Phone.Value, payload.Phone.Value.IsVerified);
      user.SetPhone(phone, actorId);
    }
  }

  private static void UpdatePersonalInformation(UpdateUserPayload payload, User user)
  {
    if (payload.FirstName != null)
    {
      user.FirstName = PersonName.TryCreate(payload.FirstName.Value);
    }
    if (payload.MiddleName != null)
    {
      user.MiddleName = PersonName.TryCreate(payload.MiddleName.Value);
    }
    if (payload.LastName != null)
    {
      user.LastName = PersonName.TryCreate(payload.LastName.Value);
    }
    if (payload.Nickname != null)
    {
      user.Nickname = PersonName.TryCreate(payload.Nickname.Value);
    }

    if (payload.Birthdate != null)
    {
      user.Birthdate = payload.Birthdate.Value;
    }
    if (payload.Gender != null)
    {
      user.Gender = Gender.TryCreate(payload.Gender.Value);
    }
    if (payload.Locale != null)
    {
      user.Locale = Locale.TryCreate(payload.Locale.Value);
    }
    if (payload.TimeZone != null)
    {
      user.TimeZone = TimeZone.TryCreate(payload.TimeZone.Value);
    }

    if (payload.Picture != null)
    {
      user.Picture = Url.TryCreate(payload.Picture.Value);
    }
    if (payload.Profile != null)
    {
      user.Profile = Url.TryCreate(payload.Profile.Value);
    }
    if (payload.Website != null)
    {
      user.Website = Url.TryCreate(payload.Website.Value);
    }
  }
}
