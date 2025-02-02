using FluentValidation.Results;
using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Passwords;

public interface IPasswordManager // TODO(fpion): remove settings whenever possible
{
  Password Create(IPasswordSettings settings, string password);

  Password Decode(string password);

  Password Generate(int length, out string password);
  Password Generate(string characters, int length, out string password);

  Password GenerateBase64(int length, out string password);

  ValidationResult Validate(IPasswordSettings settings, string password);
  void ValidateAndThrow(IPasswordSettings settings, string password);

  Password ValidateAndCreate(IPasswordSettings? settings, string password);
}
