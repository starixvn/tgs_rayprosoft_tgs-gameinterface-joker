using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class DomainGetWhitelistServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new DomainGetWhitelistQuery
		{
			ApiUrl = new Uri("https://w.api788.net/"),
			AppId = "THDT",
			SecretKey = "s5gawreoto5e1"
		};

		var result = new JsonObject
		{
			["Domain"] = new JsonArray {
				"stg - t1.rpgp.cc",
				"dev - t1.rpgp.cc"
			}
		};
		_ = fakeGameApiClient.DomainGetWhitelistAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			appId: Arg.Is(query.AppId),
			secretKey: Arg.Is(query.SecretKey),
			cancellationToken: Arg.Any<CancellationToken>())
			 .Returns(result);

		var client = new DomainGetWhitelistService(
			fakeGameApiClient);

		var actual = await client.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(result.ToJsonString(), actual.ToJsonString());
	}
}
