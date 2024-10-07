using System.Text.Json.Nodes;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class DomainGetWhitelistService(
	IGameApiClient client) : IGetService<DomainGetWhitelistQuery, JsonNode>
{
	public ValueTask<JsonNode> GetAsync(DomainGetWhitelistQuery query, CancellationToken cancellationToken = default)
	=> client.DomainGetWhitelistAsync(query.ApiUrl, query.AppId, query.SecretKey, cancellationToken);
}
