namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record GameRecordQuery(
    Uri ApiUrl,
    string AppId,
    string SecretKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string NextId);
