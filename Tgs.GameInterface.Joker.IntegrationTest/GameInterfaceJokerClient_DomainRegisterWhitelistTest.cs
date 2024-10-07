using System.Text.Json.Nodes;
using Grpc.Net.Client;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.ServiceUtility;
using NSubstitute.Routing.AutoValues;

namespace Tgs.GameInterface.Joker.IntegrationTest;
public class GameInterfaceJokerClient_DomainRegisterWhitelistTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var domains = new string[] { "dev-t1.rpgp.cc", "stg-t1.rpgp.cc" };

		var result = new JsonObject
		{

			["Status"] = "OK"
		};

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<DomainRegisterWhitelistQuery, JsonNode>>();

			var query = new DomainRegisterWhitelistQuery
			{
				ApiUrl = apiUrl,
				AppId = appId,
				SecretKey = secretKey,
				Domains = domains
			};

			_ = fakeService.GetAsync(
				query: Arg.Is<DomainRegisterWhitelistQuery>(compare =>
				 compare.ApiUrl == apiUrl &&
				 compare.AppId == appId &&
				 compare.SecretKey == secretKey &&
				 compare.Domains.Count() == 2),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

			_ = services.AddTransient(_ => fakeService);
		}));

		var sut = new Client.GameInterfaceJokerClient(new Protos.GameInterfaceJoker.GameInterfaceJokerClient(
			GrpcChannel.ForAddress(
				address: web.Server.BaseAddress,
				channelOptions: new GrpcChannelOptions { HttpHandler = web.Server.CreateHandler() })));

		var actual = await sut.DomainRegisterWhitelistAsync(apiUrl, appId, secretKey, domains);

		Assert.NotNull(actual);
		Assert.Equal(result.ToJsonString(), actual.Resp);
	}
}
