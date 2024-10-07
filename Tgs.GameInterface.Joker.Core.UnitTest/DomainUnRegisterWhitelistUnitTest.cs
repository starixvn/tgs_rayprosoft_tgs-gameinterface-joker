using System.Net;

using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.Joker.Core.UnitTest;

public class DomainUnRegisterWhitelistUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var domains = new string[] { "dev-t1.rpgp.cc", "stg-t1.rpgp.cc" };

		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeSignatureUtensil = Substitute.For<ISignatureUtensil>();

		var currentTime = DateTimeOffset.FromUnixTimeSeconds(1728263928);
		_ = fakeTimeProvider
			.GetUtcNow()
			.Returns(currentTime);

		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "UDS");
				writer.WriteString("Domain", string.Join(",", domains));
				writer.WriteNumber("Timestamp", currentTime.ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = "V52uQSC0Uzs8yjq0I6E1V/GZVdw=";
		_ = fakeSignatureUtensil
			.GenerateSignature(
			   source: Arg.Any<JsonNode>(),
			   secretKey: Arg.Is(secretKey))
			.Returns(signature);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		var respondContent = new JsonObject
		{
			["Status"] = "OK"
		};

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url)
			.WithQueryString("appid", appId)
			.WithQueryString("signature", signature)
			.WithContent(await content.ReadAsStringAsync())
			.Respond(HttpStatusCode.OK, new StringContent(respondContent.ToJsonString()));

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			fakeLog,
			fakeTimeProvider,
			fakeSignatureUtensil,
			fakeHttpClient);

		var actual = await client.DomainUnRegisterWhitelistAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			domains: domains);

		Assert.NotNull(actual);
		Assert.Equal(respondContent.ToJsonString(), actual.ToJsonString());
	}
}
