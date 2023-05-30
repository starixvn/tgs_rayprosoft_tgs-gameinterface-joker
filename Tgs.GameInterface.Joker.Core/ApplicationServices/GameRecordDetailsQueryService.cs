using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class GameRecordDetailsQueryService : IGetService<GameRecordDetailsQuery, string>
{
    private readonly IGameApiClient _client;

    public GameRecordDetailsQueryService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<string> GetAsync(GameRecordDetailsQuery query, CancellationToken cancellationToken = default)
        => _client
        .RetrieveHistoryUrlAsync(
            apiUrl: query.ApiUrl,
            appId: query.AppId,
            secretKey: query.SecretKey,
            oCode: query.ExternalRoundId,
            gameType: "Game",
            language: query.Language,
            cancellationToken: cancellationToken);
}
