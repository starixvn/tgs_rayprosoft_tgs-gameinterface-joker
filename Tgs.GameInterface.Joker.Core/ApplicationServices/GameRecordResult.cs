namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record GameRecordResult(
    string NextId,
    IEnumerable<GameRecordInfo> GameRecordInfos);
