using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class GameTokenGetService(
	IGameApiClient client) : IGetService<GameTokenQuery, string>
{
	public ValueTask<string> GetAsync(GameTokenQuery query, CancellationToken cancellationToken = default)
		=> client.PlayGameAsync(
			apiUrl: query.ApiUrl,
			appId: query.AppId,
			secretKey: query.SecretKey,
			username: query.PlayerCode,
			cancellationToken: cancellationToken);
}
