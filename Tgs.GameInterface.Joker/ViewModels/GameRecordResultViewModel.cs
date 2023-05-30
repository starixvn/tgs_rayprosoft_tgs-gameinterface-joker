namespace Tgs.GameInterface.Joker.ViewModels;

public record GameRecordResultViewModel(
    string NextId,
    IEnumerable<GameRecordInfoViewModel> GameRecordInfos);
