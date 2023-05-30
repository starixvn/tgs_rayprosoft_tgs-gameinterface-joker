using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class GameRecordQueryService : IGetService<GameRecordQuery, GameRecordResult>
{
    private readonly IGameApiClient _client;

    public GameRecordQueryService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameRecordResult> GetAsync(
        GameRecordQuery query,
         CancellationToken cancellationToken = default)
    {
        var result = await _client
            .RetrieveTransactionsByMinutesAsync(
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
