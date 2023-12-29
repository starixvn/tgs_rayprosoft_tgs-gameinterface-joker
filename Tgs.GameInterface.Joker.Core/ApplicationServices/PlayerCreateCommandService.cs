using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class PlayerCreateCommandService(
    IGameApiClient client) : ICommandService<PlayerCreateCommand>
{
    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
        => client.CreateUserAsync(
            apiUrl: command.ApiUrl,
            appId: command.AppId,
            secretKey: command.SecretKey,
            username: command.PlayerCode,
            cancellationToken: cancellationToken);
}
