namespace Tgs.GameInterface.Joker.Core.Models;

public record RetrieveTransactionsResult(
    string NextId,
    IEnumerable<TransactionResult> Transactions);
