using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

internal static class KrakenContextExtensions
{
  public static async Task<LanguageEntity> FindLanguageAsync(this KrakenContext context, LanguageId id, CancellationToken cancellationToken)
  {
    return await context.Languages.SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'StreamId={id}' could not be found.");
  }

  public static async Task<RealmEntity?> FindRealmAsync(this KrakenContext context, StreamId id, CancellationToken cancellationToken)
  {
    Tuple<RealmId?, Guid> ids = IdHelper.Decode(id);
    if (!ids.Item1.HasValue)
    {
      return null;
    }

    Guid realmId = ids.Item1.Value.ToGuid();
    return await context.Realms.SingleOrDefaultAsync(x => x.Id == realmId, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'Id={realmId}' could not be found.");
  }

  public static async Task<RoleEntity> FindRoleAsync(this KrakenContext context, RoleId id, CancellationToken cancellationToken)
  {
    return await context.Roles.SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The role entity 'StreamId={id}' could not be found.");
  }

  public static async Task<UserEntity> FindUserAsync(this KrakenContext context, UserId id, CancellationToken cancellationToken)
  {
    return await context.Users.SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity 'StreamId={id}' could not be found.");
  }
}
