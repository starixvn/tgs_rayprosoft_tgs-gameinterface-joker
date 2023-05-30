using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class PlayerCreateCommandService : ICommandService<PlayerCreateCommand>
{
    private readonly IGameApiClient _client;

    public PlayerCreateCommandService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
        => _client
        .CreateUserAsync(
            apiUrl: command.ApiUrl,
            appId: command.AppId,
            secretKey: command.SecretKey,
            username: command.PlayerCode,
            cancellationToken: cancellationToken);
}
