using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.Joker.Core.UnitTest;

public class WithdrawUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var username = "4ucmqplxb029";
		var requestId = "01886638f0830000000107e78f2b8c75";
		var amount = 123m;

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
				writer.WriteString("Method", "TC");
				writer.WriteString("Username", username);
				writer.WriteString("RequestID", requestId);
				writer.WriteNumber("Amount", -amount);
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

		var respondContent = "{\"RequestID\":\"01886638f0830000000107e78f2b8c75\",\"Username\":\"4UCMQPLXB029\",\"Time\":\"2023-05-29T14:36:22.3405013\\u002B08:00\",\"Credit\":0.0,\"BeforeCredit\":123.0,\"OutstandingCredit\":0.0}";

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

		var actual = await client.WithdrawAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			username: username,
			requestId: requestId,
			amount: amount);

		Assert.NotNull(actual);
		Assert.Equal(expected: requestId, actual: actual.RequestId);
		Assert.Equal(expected: respondContent, actual: actual.Resp);
	}
}
