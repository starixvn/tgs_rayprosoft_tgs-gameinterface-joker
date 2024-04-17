namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record FundInQuery(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode,
	string TransactionId,
	decimal Amount);
