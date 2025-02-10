using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Contracts.Contents;

public class ContentTypeModel : AggregateModel
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int FieldCount { get; set; }
  public List<FieldDefinitionModel> Fields { get; set; } = [];

  public RealmModel? Realm { get; set; }
}
