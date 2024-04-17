using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class FundStatusGetService(IGameApiClient client) : IGetService<FundStatusQuery, bool>
{
	public ValueTask<bool> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
		=> client.VerifyTransferCreditAsync(
			apiUrl: query.ApiUrl,
			appId: query.AppId,
			secretKey: query.SecretKey,
			requestId: query.TransactionId,
			cancellationToken: cancellationToken);
}
