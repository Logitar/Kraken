namespace Logitar.Kraken.Core;

public class IdAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified ID is already used.";

  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
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
      error.Data[nameof(Id)] = Id;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public IdAlreadyUsedException(Guid id, string propertyName) : base(BuildMessage(id, propertyName))
  {
    Id = id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid id, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
