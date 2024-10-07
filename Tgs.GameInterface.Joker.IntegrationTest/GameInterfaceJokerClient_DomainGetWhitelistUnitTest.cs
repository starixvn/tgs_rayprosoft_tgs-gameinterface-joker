using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.IntegrationTest;

public class GameInterfaceJokerClient_DomainGetWhitelistUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";

		var result = new JsonObject
		{
			["Domain"] = new JsonArray {
				"stg - t1.rpgp.cc",
				"dev - t1.rpgp.cc"
			}
		};

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<DomainGetWhitelistQuery, JsonNode>>();

			var query = new DomainGetWhitelistQuery
			{
				ApiUrl = apiUrl,
				AppId = appId,
				SecretKey = secretKey
			};

			_ = fakeService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

			_ = services.AddTransient(_ => fakeService);
		}));

		var sut = new Client.GameInterfaceJokerClient(new Protos.GameInterfaceJoker.GameInterfaceJokerClient(
			GrpcChannel.ForAddress(
				address: web.Server.BaseAddress,
				channelOptions: new GrpcChannelOptions { HttpHandler = web.Server.CreateHandler() })));

		var actual = await sut.DomainGetWhitelistAsync(apiUrl, appId, secretKey);

		Assert.NotNull(actual);
		Assert.Equal(result.ToJsonString(), actual.Resp);
	}
}
