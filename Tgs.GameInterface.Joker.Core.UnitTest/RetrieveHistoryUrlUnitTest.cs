using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.Joker.Core.UnitTest;

public class RetrieveHistoryUrlUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		string oCode = "qppiorex5ax4a";
		string gameType = "Game";
		string language = "zh";

		var fakeSystemClock = Substitute.For<ISystemClock>();
		var fakeSignatureUtensil = Substitute.For<ISignatureUtensil>();

		var currentTime = DateTimeOffset.Parse("2023-05-29T11:24:18+08:00");
		_ = fakeSystemClock
			.CurrentTime
			.Returns(currentTime);

		System.Text.Json.Nodes.JsonNode parameters;

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "History");
				writer.WriteString("OCode", oCode);
				writer.WriteString("Type", gameType);
				writer.WriteString("Language", language);
				writer.WriteNumber("Timestamp", currentTime.ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = "SCGv1Twtmv8L0FlmTnXfb7Gj+os=";
		_ = fakeSignatureUtensil
			.GenerateSignature(
			   source: Arg.Any<System.Text.Json.Nodes.JsonNode>(),
			   secretKey: Arg.Is(secretKey))
			.Returns(signature);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		var respondContent = "{\"Url\":\"//hs.gwp6868.net/History?Signature=XSwHzoWB0ko2HsbRjwLq8YTzjgU%3d&Key=qppiorex5ax4a&Timestamp=1685343480524&Lang=zh&TransactionOCode=xey6kurg58ufq\"}";

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url)
				.WithQueryString("appid", appId)
				.WithQueryString("signature", signature)
				.WithContent(await content.ReadAsStringAsync())
				.Respond(HttpStatusCode.OK, new StringContent(respondContent));

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			fakeLog,
			fakeSystemClock,
			fakeSignatureUtensil,
			fakeHttpClient);

		var actual = await client.RetrieveHistoryUrlAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			oCode: oCode,
			gameType: gameType,
			language: language);

		Assert.NotNull(actual);
		Assert.Equal("//hs.gwp6868.net/History?Signature=XSwHzoWB0ko2HsbRjwLq8YTzjgU%3d&Key=qppiorex5ax4a&Timestamp=1685343480524&Lang=zh&TransactionOCode=xey6kurg58ufq", actual);
	}
}
