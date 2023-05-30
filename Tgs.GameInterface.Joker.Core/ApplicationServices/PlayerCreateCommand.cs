namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record PlayerCreateCommand(
    Uri ApiUrl,
    string AppId,
    string SecretKey,
    string PlayerCode);
