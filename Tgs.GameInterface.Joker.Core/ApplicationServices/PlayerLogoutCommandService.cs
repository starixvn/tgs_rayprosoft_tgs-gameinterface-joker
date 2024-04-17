using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class PlayerLogoutCommandService(
	IGameApiClient client) : ICommandService<PlayerLogoutCommand>
{
	public ValueTask ExecuteAsync(PlayerLogoutCommand command, CancellationToken cancellationToken = default)
		=> client.SignOutUserAsync(
			apiUrl: command.ApiUrl,
			appId: command.AppId,
			secretKey: command.SecretKey,
			username: command.PlayerCode,
			cancellationToken: cancellationToken);
}
