using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Core.Messages;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class RecipientEntity
{
  public int RecipientId { get; private set; }

  public MessageEntity? Message { get; private set; }
  public int MessageId { get; private set; }

  public RecipientType Type { get; private set; }

  public string? Address { get; private set; }
  public string? DisplayName { get; private set; }

  public string? PhoneNumber { get; private set; }

  public UserEntity? User { get; private set; }
  public int? UserId { get; private set; }
  public string? UserUniqueName { get; private set; }
  public string? UserEmailAddress { get; private set; }
  public string? UserFullName { get; private set; }
  public string? UserPicture { get; private set; }

  public RecipientEntity(MessageEntity message, UserEntity? user, Recipient recipient)
  {
    Message = message;
    MessageId = message.MessageId;

    Type = recipient.Type;

    Address = recipient.Address;
    DisplayName = recipient.DisplayName;

    PhoneNumber = recipient.PhoneNumber;

    if (user != null)
    {
      User = user;
      UserId = user.UserId;
      UserUniqueName = user.UniqueName;
      UserEmailAddress = user.EmailAddress;
      UserFullName = user.FullName;
      UserPicture = user.Picture;
    }
  }

  private RecipientEntity()
  {
  }

  // TODO(fpion): Equals, GetHashCode, ToString
}
