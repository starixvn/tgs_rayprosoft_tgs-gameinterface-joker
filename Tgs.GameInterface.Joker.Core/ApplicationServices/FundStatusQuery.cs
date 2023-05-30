namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record FundStatusQuery(
    Uri ApiUrl,
    string AppId,
    string SecretKey,
    string TransactionId);
