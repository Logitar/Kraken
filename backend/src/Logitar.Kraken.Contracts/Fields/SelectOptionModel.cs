namespace Logitar.Kraken.Contracts.Fields;

public record SelectOptionModel : ISelectOption
{
  public bool IsDisabled { get; set; }
  public string Text { get; set; } = string.Empty;
  public string? Label { get; set; }
  public string? Value { get; set; }

  public SelectOptionModel()
  {
  }

  public SelectOptionModel(ISelectOption option)
  {
    IsDisabled = option.IsDisabled;
    Text = option.Text;
    Label = option.Label;
    Value = option.Value;
  }
}
