namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record FundOutQuery(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode,
	string TransactionId,
	decimal Amount);
