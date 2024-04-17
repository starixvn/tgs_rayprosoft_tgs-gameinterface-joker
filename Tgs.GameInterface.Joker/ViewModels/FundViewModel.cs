namespace Tgs.GameInterface.Joker.ViewModels;

public record FundViewModel(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string PlayerCode,
	string TransactionId,
	decimal Amount);
