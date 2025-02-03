namespace Logitar.Kraken.Contracts.Fields;

public interface IRichTextProperties
{
  string ContentType { get; }
  int? MinimumLength { get; }
  int? MaximumLength { get; }
}
