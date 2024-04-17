namespace Tgs.GameInterface.Joker.Core.Models;

public record GetCreditResult(
	bool IsPlayerExist,
	decimal Credit,
	decimal OutstandingCredit);
