namespace Tgs.GameInterface.Joker.ViewModels;

public record GameRecordViewModel(
	Uri ApiUrl,
	string AppId,
	string SecretKey,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime,
	string NextId);
