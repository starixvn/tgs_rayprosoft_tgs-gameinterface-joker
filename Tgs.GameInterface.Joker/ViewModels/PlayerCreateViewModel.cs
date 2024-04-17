namespace Tgs.GameInterface.Joker.ViewModels;

public record PlayerCreateViewModel(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode);
