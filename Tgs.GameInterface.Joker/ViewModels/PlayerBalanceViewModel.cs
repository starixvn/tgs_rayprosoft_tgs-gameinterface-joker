namespace Tgs.GameInterface.Joker.ViewModels;

public record PlayerBalanceViewModel(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode);
