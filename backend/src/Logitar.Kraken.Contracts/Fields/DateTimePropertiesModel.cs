namespace Logitar.Kraken.Contracts.Fields;

public record DateTimePropertiesModel : IDateTimeProperties
{
  public DateTime? MinimumValue { get; set; }
  public DateTime? MaximumValue { get; set; }

  public DateTimePropertiesModel()
  {
  }

  public DateTimePropertiesModel(IDateTimeProperties dateTime)
  {
    MinimumValue = dateTime.MinimumValue;
    MaximumValue = dateTime.MaximumValue;
  }
}
