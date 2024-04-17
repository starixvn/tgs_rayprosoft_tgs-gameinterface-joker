using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class GameRecordDetailsQueryService(IGameApiClient client) : IGetService<GameRecordDetailsQuery, string>
{
	public ValueTask<string> GetAsync(GameRecordDetailsQuery query, CancellationToken cancellationToken = default)
		=> client.RetrieveHistoryUrlAsync(
			apiUrl: query.ApiUrl,
			appId: query.AppId,
			secretKey: query.SecretKey,
			oCode: query.ExternalRoundId,
			gameType: "Game",
			language: query.Language,
			cancellationToken: cancellationToken);
}
