﻿using FluentValidation;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Users.Commands;

/// <exception cref="CustomIdentifierAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record SaveUserIdentifierCommand(Guid Id, string Key, SaveUserIdentifierPayload Payload) : Activity, IRequest<UserModel?>;

internal class SaveUserIdentifierCommandHandler : IRequestHandler<SaveUserIdentifierCommand, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SaveUserIdentifierCommandHandler(
    IApplicationContext applicationContext,
    IUserManager userManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(SaveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    CustomIdentifierModel identifier = new(command.Key, command.Payload.Value);
    new CustomIdentifierValidator().ValidateAndThrow(identifier);

    UserId userId = new(command.Id, _applicationContext.RealmId);
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    Identifier key = new(identifier.Key);
    CustomIdentifier value = new(identifier.Value);
    user.SetCustomIdentifier(key, value, _applicationContext.ActorId);

    await _userManager.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
