namespace Logitar.Kraken.Core.Roles;

public class RolesNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified roles could not be found.";

  public IReadOnlyCollection<string> Roles
  {
    get => (IReadOnlyCollection<string>)Data[nameof(Roles)]!;
    private set => Data[nameof(Roles)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(Roles)] = Roles;
      return error;
    }
  }

  public RolesNotFoundException(IEnumerable<string> roles) : base(BuildMessage(roles))
  {
    Roles = roles.ToList().AsReadOnly();
  }

  private static string BuildMessage(IEnumerable<string> roles)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(Roles)).Append(':').AppendLine();
    foreach (string role in roles)
    {
      message.Append(" - ").AppendLine(role);
    }
    return message.ToString();
  }
}
