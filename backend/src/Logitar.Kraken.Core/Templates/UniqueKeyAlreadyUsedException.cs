namespace Logitar.Kraken.Core.Templates;

public class UniqueKeyAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique key is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid TemplateId
  {
    get => (Guid)Data[nameof(TemplateId)]!;
    private set => Data[nameof(TemplateId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueKey
  {
    get => (string)Data[nameof(UniqueKey)]!;
    private set => Data[nameof(UniqueKey)] = value;
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
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(TemplateId)] = TemplateId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueKey)] = UniqueKey;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueKeyAlreadyUsedException(Template template, TemplateId conflictId) : base(BuildMessage(template, conflictId))
  {
    RealmId = template.RealmId?.ToGuid();
    TemplateId = template.EntityId;
    ConflictId = conflictId.EntityId;
    UniqueKey = template.UniqueKey.Value;
    PropertyName = nameof(Template.UniqueKey);
  }

  private static string BuildMessage(Template template, TemplateId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), template.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(TemplateId), template.EntityId)
    .AddData(nameof(ConflictId), conflictId.EntityId)
    .AddData(nameof(UniqueKey), template.UniqueKey.Value)
    .AddData(nameof(PropertyName), nameof(template.UniqueKey))
    .Build();
}
