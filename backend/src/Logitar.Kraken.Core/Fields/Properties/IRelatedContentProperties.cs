using Logitar.Kraken.Core.Contents;

namespace Logitar.Kraken.Core.Fields.Properties;

public interface IRelatedContentProperties
{
  ContentTypeId ContentTypeId { get; }
  bool IsMultiple { get; }
}
