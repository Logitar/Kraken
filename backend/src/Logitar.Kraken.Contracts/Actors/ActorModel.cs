using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Contracts.Actors;

public class ActorModel
{
  public Guid Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = "System";
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public ActorModel()
  {
  }

  public ActorModel(ApiKeyModel apiKey)
  {
    Id = apiKey.Id;
    Type = ActorType.ApiKey;

    DisplayName = apiKey.Name;
  }

  public ActorModel(UserModel user)
  {
    Id = user.Id;
    Type = ActorType.User;

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }

  public override bool Equals(object obj) => obj is ActorModel actor && actor.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder actor = new();
    actor.Append(DisplayName);
    if (EmailAddress != null)
    {
      actor.Append(" <").Append(EmailAddress).Append('>');
    }
    actor.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return actor.ToString();
  }
}
