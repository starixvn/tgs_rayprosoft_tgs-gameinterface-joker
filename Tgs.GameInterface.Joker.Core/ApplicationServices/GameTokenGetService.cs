using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class GameTokenGetService : IGetService<GameTokenQuery, string>
{
    private readonly IGameApiClient _client;

    public GameTokenGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<string> GetAsync(GameTokenQuery query, CancellationToken cancellationToken = default)
        => _client.PlayGameAsync(
            apiUrl: query.ApiUrl,
            appId: query.AppId,
            secretKey: query.SecretKey,
            username: query.PlayerCode,
            cancellationToken: cancellationToken);
}
