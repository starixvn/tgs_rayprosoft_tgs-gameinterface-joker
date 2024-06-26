using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class FundInGetService(
	IGameApiClient client) : IGetService<FundInQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client
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
			Resp: result.Resp);
	}
}
