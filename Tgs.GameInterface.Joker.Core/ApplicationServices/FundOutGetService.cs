using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class FundOutGetService : IGetService<FundOutQuery, FundResult>
{
    private readonly IGameApiClient _client;

    public FundOutGetService(
        IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _client
             .DepositAsync(
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
