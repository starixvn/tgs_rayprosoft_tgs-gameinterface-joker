namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

public record GameRecordInfo(
	string ExternalRoundId,
	string ExternalGameId,
	string PlayerCode,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal Settle,
	decimal WinLoss,
	string AdditionalData);
