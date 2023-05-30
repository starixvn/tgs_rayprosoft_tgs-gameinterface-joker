namespace Tgs.GameInterface.Joker.ViewModels;

public record PlayerLogoutViewModel(
    Uri ApiUrl,
    string AppId,
    string SecretKey,
    string PlayerCode);
