using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class FundInGetService : IGetService<FundInQuery, FundResult>
{
    private readonly IGameApiClient _client;

    public FundInGetService(
        IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _client
             .WithdrawAsync(
                 apiUrl: query.ApiUrl,
                 appId: query.AppId,
                 secretKey: query.SecretKey,
                 username: query.PlayerCode,
                 requestId: query.TransactionId,
                 amount: query.Amount,
                 cancellationToken: cancellationToken)
             .ConfigureAwait(false);

        return new FundResult(
            RequestId: result.RequestId,
            Res: result.Res);
    }
}
