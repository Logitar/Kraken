using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Contracts.Contents;

public class ContentModel : AggregateModel
{
  public ContentTypeModel ContentType { get; set; } = new();

  public ContentLocaleModel Invariant { get; set; }
  public List<ContentLocaleModel> Locales { get; set; } = [];

  public RealmModel? Realm { get; set; }

  public ContentModel()
  {
    Invariant = new(this);
  }

  public override string ToString() => $"{Invariant} | {base.ToString()}";
}
