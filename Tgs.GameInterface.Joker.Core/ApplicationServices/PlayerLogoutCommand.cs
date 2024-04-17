namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record PlayerLogoutCommand(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode);
