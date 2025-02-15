using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Web.Models.Account;

namespace Logitar.Kraken.Web.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken = default);
  Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken = default);
  Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);
}
