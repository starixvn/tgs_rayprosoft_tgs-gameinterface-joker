namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record GameRecordDetailsQuery(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	string ExternalRoundId,
	string Language);
