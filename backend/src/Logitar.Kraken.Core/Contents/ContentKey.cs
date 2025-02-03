namespace Logitar.Kraken.Core.Contents;

public record ContentKey(Guid ContentTypeId, Guid? LanguageId, string UniqueName);
