namespace Tgs.GameInterface.Joker.ViewModels;

public record GameTokenQueryViewModel(
    Uri ApiUrl,
    string AppId,
    string SecretKey,
    string PlayerCode);
