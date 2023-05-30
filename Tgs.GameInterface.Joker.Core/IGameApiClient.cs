using Tgs.GameInterface.Joker.Core.Models;

namespace Tgs.GameInterface.Joker.Core;

public interface IGameApiClient
{
    public ValueTask CreateUserAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default);

    public ValueTask<TransferCreditResult> DepositAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        string requestId,
        decimal amount,
        CancellationToken cancellationToken = default);

    public ValueTask<GetCreditResult> GetCreditAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default);

    public ValueTask<string> PlayGameAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default);

    public ValueTask<string> RetrieveHistoryUrlAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string oCode,
        string gameType,
        string language,
        CancellationToken cancellationToken = default);

    public ValueTask<RetrieveTransactionsResult> RetrieveTransactionsByMinutesAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string nextId,
        CancellationToken cancellationToken = default);

    public ValueTask SignOutUserAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default);

    public ValueTask<bool> VerifyTransferCreditAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string requestId,
        CancellationToken cancellationToken = default);

    public ValueTask<TransferCreditResult> WithdrawAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        string requestId,
        decimal amount,
        CancellationToken cancellationToken = default);
}
