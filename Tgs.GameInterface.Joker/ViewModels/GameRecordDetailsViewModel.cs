namespace Tgs.GameInterface.Joker.ViewModels;

public record GameRecordDetailsViewModel(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string ExternalRoundId,
	string Language);
