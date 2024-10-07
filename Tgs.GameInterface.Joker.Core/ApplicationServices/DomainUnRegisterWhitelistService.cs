using System.Text.Json.Nodes;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;

internal class DomainUnRegisterWhitelistService(
	IGameApiClient client) : IGetService<DomainUnRegisterWhitelistQuery, JsonNode>
{
	public ValueTask<JsonNode> GetAsync(DomainUnRegisterWhitelistQuery query, CancellationToken cancellationToken = default)
	=> client.DomainUnRegisterWhitelistAsync(query.ApiUrl, query.AppId, query.SecretKey, query.Domains, cancellationToken);
}
