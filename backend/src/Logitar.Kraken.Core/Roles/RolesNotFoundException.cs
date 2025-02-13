namespace Logitar.Kraken.Core.Roles;

public class RolesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified roles could not be found.";

  public IReadOnlyCollection<string> Roles
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Roles)]!;
    private set => Data[nameof(Roles)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(Roles)] = Roles;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RolesNotFoundException(IEnumerable<string> roles, string propertyName) : base(BuildMessage(roles, propertyName))
  {
    Roles = roles.ToList().AsReadOnly();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<string> roles, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(Roles)).Append(':').AppendLine();
    foreach (string role in roles)
    {
      message.Append(" - ").AppendLine(role);
    }
    return message.ToString();
  }
}
