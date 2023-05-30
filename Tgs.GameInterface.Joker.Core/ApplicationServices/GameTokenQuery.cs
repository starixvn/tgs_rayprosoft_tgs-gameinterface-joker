namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record GameTokenQuery(
    Uri ApiUrl,
    string AppId,
    string SecretKey,
    string PlayerCode);
