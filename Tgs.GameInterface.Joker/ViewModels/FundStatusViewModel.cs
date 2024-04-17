namespace Tgs.GameInterface.Joker.ViewModels;

public record FundStatusViewModel(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string TransactionId);
