namespace Tgs.GameInterface.Joker.Core.Models;

public record TransactionResult(
    string OCode,
    string Username,
    string GameCode,
    string Description,
    decimal Amount,
    decimal Result,
    DateTimeOffset Time,
    string AdditionalData);
