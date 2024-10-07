using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class DomainUnRegisterWhitelistServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new DomainUnRegisterWhitelistQuery
		{
			ApiUrl = new Uri("https://w.api788.net/"),
			AppId = "THDT",
			SecretKey = "s5gawreoto5e1",
			Domains = ["dev-t1.rpgp.cc", "stg-t1.rpgp.cc"]
		};

		var result = new JsonObject
		{
			["Status"] = "OK"
		};
		_ = fakeGameApiClient.DomainUnRegisterWhitelistAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			appId: Arg.Is(query.AppId),
			secretKey: Arg.Is(query.SecretKey),
			domains: Arg.Is(query.Domains),
			cancellationToken: Arg.Any<CancellationToken>())
			 .Returns(result);

		var client = new DomainUnRegisterWhitelistService(
			fakeGameApiClient);

		var actual = await client.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(result.ToJsonString(), actual.ToJsonString());
	}
}
