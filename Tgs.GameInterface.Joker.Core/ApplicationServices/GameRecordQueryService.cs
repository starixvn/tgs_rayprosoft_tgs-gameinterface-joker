using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class GameRecordQueryService(
	IGameApiClient client) : IGetService<GameRecordQuery, GameRecordResult>
{
	public async ValueTask<GameRecordResult> GetAsync(
		GameRecordQuery query,
		 CancellationToken cancellationToken = default)
	{
		var result = await client.RetrieveTransactionsByMinutesAsync(
			apiUrl: query.ApiUrl,
			appId: query.AppId,
			secretKey: query.SecretKey,
			startDate: query.StartTime,
			endDate: query.EndTime,
			nextId: query.NextId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameRecordResult(
			NextId: result.NextId,
			GameRecordInfos: result.Transactions.Select(x => new GameRecordInfo(
					ExternalRoundId: x.OCode,
					ExternalGameId: x.GameCode,
					PlayerCode: x.Username.ToLower(),
					ExternalCreatedOn: x.Time,
					ExternalClosedOn: x.Time,
					Bet: x.Amount,
					Settle: x.Result,
					WinLoss: x.Result - x.Amount,
					AdditionalData: x.AdditionalData)));
	}
}
