namespace Logitar.Kraken.Contracts.Fields;

public record NumberPropertiesModel : INumberProperties
{
  public double? MinimumValue { get; set; }
  public double? MaximumValue { get; set; }
  public double? Step { get; set; }

  public NumberPropertiesModel()
  {
  }

  public NumberPropertiesModel(INumberProperties number)
  {
    MinimumValue = number.MinimumValue;
    MaximumValue = number.MaximumValue;
    Step = number.Step;
  }
}
