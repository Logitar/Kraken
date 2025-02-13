using FluentValidation.Results;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Passwords;

internal class PasswordManager : IPasswordManager // TODO(fpion): implement
{
  public Password Create(IPasswordSettings settings, string password)
  {
    throw new NotImplementedException();
  }

  public Password Decode(string value)
  {
    throw new NotImplementedException();
  }

  public Password Generate(int length, out string password)
  {
    throw new NotImplementedException();
  }
  public Password Generate(string characters, int length, out string password)
  {
    throw new NotImplementedException();
  }

  public Password GenerateBase64(int length, out string password)
  {
    throw new NotImplementedException();
  }

  public ValidationResult Validate(IPasswordSettings settings, string password)
  {
    throw new NotImplementedException();
  }
  public void ValidateAndThrow(IPasswordSettings settings, string password)
  {
    throw new NotImplementedException();
  }

  public Password ValidateAndCreate(IPasswordSettings? settings, string password)
  {
    throw new NotImplementedException();
  }
}
