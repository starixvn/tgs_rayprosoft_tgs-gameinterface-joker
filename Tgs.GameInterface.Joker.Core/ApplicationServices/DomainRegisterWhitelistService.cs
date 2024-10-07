using System.Text.Json.Nodes;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class DomainRegisterWhitelistService(
	IGameApiClient client) : IGetService<DomainRegisterWhitelistQuery, JsonNode>
{
	public ValueTask<JsonNode> GetAsync(DomainRegisterWhitelistQuery query, CancellationToken cancellationToken = default)
	=> client.DomainRegisterWhitelistAsync(query.ApiUrl, query.AppId, query.SecretKey, query.Domains, cancellationToken);
}
