namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record PlayerBalanceQuery(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode);
