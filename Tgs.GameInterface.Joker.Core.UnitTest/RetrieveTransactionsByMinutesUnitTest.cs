using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.Joker.Core.UnitTest;

public class RetrieveTransactionsByMinutesUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var startDate = DateTimeOffset.Parse("2023-05-27T11:55:00+00:00");
		var endDate = DateTimeOffset.Parse("2023-05-27T12:00:00+00:00");
		var nextId = string.Empty;
		var timeZone = "Greenwich Standard Time";

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
				writer.WriteString("Method", "TSM");
				writer.WriteString("StartDate", startDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("EndDate", endDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("NextId", string.IsNullOrEmpty(nextId) ? string.Empty : nextId);
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
		queryStrings.Add("timeZone", timeZone);

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		var respondContent = "{\"data\":{\"Game\":[{\"OCode\":\"xm86m1uyyqbiq\",\"Username\":\"4UK0IHIHU5V6\",\"GameCode\":\"5m6k9j7rwspjs\",\"Description\":\"Main Spin\",\"RoundID\":\"xm86m1uyyqbiq\",\"Amount\":1.5,\"FreeAmount\":0.0,\"Result\":12.5,\"Time\":\"2023-05-27T11:59:52.0880000+00:00\",\"Details\":null,\"AppID\":\"FG6W\",\"CurrencyCode\":\"HKD\",\"Type\":\"Game\",\"TransactionOCode\":\"xj8eohjkknbiq\",\"StartBalance\":5.5,\"EndBalance\":16.5}],\"Jackpot\":[{\"OCode\":\"xm7m5c7qknbiq\",\"Username\":\"4UK0IHIHU5V6\",\"GameCode\":\"Jackpot\",\"Description\":\"Prize Drop\",\"RoundID\":\"xm8uy6zxknbiq\",\"Amount\":0.0,\"FreeAmount\":0.0,\"Result\":3.0,\"Time\":\"2023-05-27T11:57:05.0230000+00:00\",\"Details\":null,\"AppID\":\"FG6W\",\"CurrencyCode\":\"HKD\",\"Type\":\"Prize Drop\",\"TransactionOCode\":\"xm7m5c7qknbiq\",\"StartBalance\":13.0,\"EndBalance\":16.0}]},\"nextId\":\"eyIyMDIzMDUyNzE5NTkiOnsiQ3JlYXRlZFRpbWUiOiJmZTRmMTk0MC1mYzg1LTExZWQtYTcyNC1kNWEzMjY4YjlmNmIifX0=\",\"games\":[{\"GameCode\":\"5m6k9j7rwspjs\",\"GameName\":\"Roma\",\"GameType\":\"Slot\"},{\"GameCode\":\"Jackpot\",\"GameName\":\"Jackpot\",\"GameType\":\"Jackpot\"}]}";

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url)
				.WithQueryString("appid", appId)
				.WithQueryString("signature", signature)
				.WithQueryString("timeZone", timeZone)
				.WithContent(await content.ReadAsStringAsync())
				.Respond(HttpStatusCode.OK, new StringContent(respondContent));

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			fakeLog,
			fakeSystemClock,
			fakeSignatureUtensil,
			fakeHttpClient);

		var actual = await client
			.RetrieveTransactionsByMinutesAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			startDate: startDate,
			endDate: endDate,
			nextId: nextId);

		Assert.NotNull(actual);
		Assert.Equal("eyIyMDIzMDUyNzE5NTkiOnsiQ3JlYXRlZFRpbWUiOiJmZTRmMTk0MC1mYzg1LTExZWQtYTcyNC1kNWEzMjY4YjlmNmIifX0=", actual.NextId);
		Assert.True(actual.Transactions.Any());
		Assert.Equal(2, actual.Transactions.Count());
	}

	[Fact]
	public async Task Normal_Process_Jackpot()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var startDate = DateTimeOffset.Parse("2023-05-27T11:55:00+00:00");
		var endDate = DateTimeOffset.Parse("2023-05-27T12:00:00+00:00");
		var nextId = string.Empty;
		var timeZone = "Greenwich Standard Time";

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
				writer.WriteString("Method", "TSM");
				writer.WriteString("StartDate", startDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("EndDate", endDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("NextId", string.IsNullOrEmpty(nextId) ? string.Empty : nextId);
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
		queryStrings.Add("timeZone", timeZone);

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		var respondContent = "{\"data\":{\"Jackpot\":[{\"OCode\":\"xm7m5c7qknbiq\",\"Username\":\"4UK0IHIHU5V6\",\"GameCode\":\"Jackpot\",\"Description\":\"Prize Drop\",\"RoundID\":\"xm8uy6zxknbiq\",\"Amount\":0.0,\"FreeAmount\":0.0,\"Result\":3.0,\"Time\":\"2023-05-27T11:57:05.0230000+00:00\",\"Details\":null,\"AppID\":\"FG6W\",\"CurrencyCode\":\"HKD\",\"Type\":\"Prize Drop\",\"TransactionOCode\":\"xm7m5c7qknbiq\",\"StartBalance\":13.0,\"EndBalance\":16.0}]},\"nextId\":\"eyIyMDIzMDUyNzE5NTkiOnsiQ3JlYXRlZFRpbWUiOiJmZTRmMTk0MC1mYzg1LTExZWQtYTcyNC1kNWEzMjY4YjlmNmIifX0=\",\"games\":[{\"GameCode\":\"5m6k9j7rwspjs\",\"GameName\":\"Roma\",\"GameType\":\"Slot\"},{\"GameCode\":\"Jackpot\",\"GameName\":\"Jackpot\",\"GameType\":\"Jackpot\"}]}";

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url)
				.WithQueryString("appid", appId)
				.WithQueryString("signature", signature)
				.WithQueryString("timeZone", timeZone)
				.WithContent(await content.ReadAsStringAsync())
				.Respond(HttpStatusCode.OK, new StringContent(respondContent));

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			fakeLog,
			fakeSystemClock,
			fakeSignatureUtensil,
			fakeHttpClient);

		var actual = await client.RetrieveTransactionsByMinutesAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			startDate: startDate,
			endDate: endDate,
			nextId: nextId);

		Assert.NotNull(actual);
		Assert.Equal("eyIyMDIzMDUyNzE5NTkiOnsiQ3JlYXRlZFRpbWUiOiJmZTRmMTk0MC1mYzg1LTExZWQtYTcyNC1kNWEzMjY4YjlmNmIifX0=", actual.NextId);
		Assert.Equal("xm7m5c7qknbiq", actual.Transactions.Single().OCode);
		Assert.Equal("4UK0IHIHU5V6", actual.Transactions.Single().Username);
		Assert.Equal("Jackpot", actual.Transactions.Single().GameCode);
		Assert.Equal("Prize Drop", actual.Transactions.Single().Description);
		Assert.Equal(0m, actual.Transactions.Single().Amount);
		Assert.Equal(3m, actual.Transactions.Single().Result);
		Assert.Equal(DateTimeOffset.Parse("2023-05-27T11:57:05.0230000+00:00"), actual.Transactions.Single().Time);
	}

	[Fact]
	public async Task Normal_Process_Lose()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var startDate = DateTimeOffset.Parse("2023-05-29T06:36:00+00:00");
		var endDate = DateTimeOffset.Parse("2023-05-29T06:56:00+00:00");
		var nextId = string.Empty;
		var timeZone = "Greenwich Standard Time";

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
				writer.WriteString("Method", "TSM");
				writer.WriteString("StartDate", startDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("EndDate", endDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("NextId", string.IsNullOrEmpty(nextId) ? string.Empty : nextId);
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
		queryStrings.Add("timeZone", timeZone);

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		var respondContent = "{\"data\":{\"Game\":[{\"OCode\":\"qpp5fiarswx4a\",\"Username\":\"4UCMQPLXB029\",\"GameCode\":\"naagsa5ycfugq\",\"Description\":\"Main Scroll\",\"RoundID\":\"qpp5fiarswx4a\",\"Amount\":0.8,\"FreeAmount\":0.0,\"Result\":0.0,\"Time\":\"2023-05-29T06:51:42.2570000+00:00\",\"Details\":null,\"AppID\":\"THDT\",\"CurrencyCode\":\"CNY\",\"Type\":\"Game\",\"TransactionOCode\":\"xey6kurg58ufq\",\"StartBalance\":121.8,\"EndBalance\":121.0}]},\"nextId\":\"eyIyMDIzMDUyOTE0NTEiOnsiQ3JlYXRlZFRpbWUiOiI0NmJkZTU0MS1mZGVkLTExZWQtYjM4ZS1iZmVlZjQzMjQ0OWIifX0=\",\"games\":[{\"GameCode\":\"naagsa5ycfugq\",\"GameName\":\"Ancient Egypt\",\"GameType\":\"Slot\"}]}";

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url)
				.WithQueryString("appid", appId)
				.WithQueryString("signature", signature)
				.WithQueryString("timeZone", timeZone)
				.WithContent(await content.ReadAsStringAsync())
				.Respond(HttpStatusCode.OK, new StringContent(respondContent));

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			fakeLog,
			fakeSystemClock,
			fakeSignatureUtensil,
			fakeHttpClient);

		var actual = await client.RetrieveTransactionsByMinutesAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			startDate: startDate,
			endDate: endDate,
			nextId: nextId);

		Assert.NotNull(actual);
		Assert.Equal("eyIyMDIzMDUyOTE0NTEiOnsiQ3JlYXRlZFRpbWUiOiI0NmJkZTU0MS1mZGVkLTExZWQtYjM4ZS1iZmVlZjQzMjQ0OWIifX0=", actual.NextId);
		Assert.Equal("qpp5fiarswx4a", actual.Transactions.Single().OCode);
		Assert.Equal("4UCMQPLXB029", actual.Transactions.Single().Username);
		Assert.Equal("naagsa5ycfugq", actual.Transactions.Single().GameCode);
		Assert.Equal("Main Scroll", actual.Transactions.Single().Description);
		Assert.Equal(0.8m, actual.Transactions.Single().Amount);
		Assert.Equal(0m, actual.Transactions.Single().Result);
		Assert.Equal(DateTimeOffset.Parse("2023-05-29T06:51:42.2570000+00:00"), actual.Transactions.Single().Time);
	}

	[Fact]
	public async Task Normal_Process_Win()
	{
		var apiUrl = new Uri("https://w.api788.net/");
		var appId = "THDT";
		var secretKey = "s5gawreoto5e1";
		var startDate = DateTimeOffset.Parse("2023-05-29T06:36:00+00:00");
		var endDate = DateTimeOffset.Parse("2023-05-29T06:56:00+00:00");
		var nextId = string.Empty;
		var timeZone = "Greenwich Standard Time";

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
				writer.WriteString("Method", "TSM");
				writer.WriteString("StartDate", startDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("EndDate", endDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("NextId", string.IsNullOrEmpty(nextId) ? string.Empty : nextId);
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
		queryStrings.Add("timeZone", timeZone);

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		var respondContent = "{\"data\":{\"Game\":[{\"OCode\":\"qpp5ra4k5ax4a\",\"Username\":\"4UCMQPLXB029\",\"GameCode\":\"naagsa5ycfugq\",\"Description\":\"Main Scroll\",\"RoundID\":\"qpp5ra4k5ax4a\",\"Amount\":1.2,\"FreeAmount\":0.0,\"Result\":2.4,\"Time\":\"2023-05-29T06:51:31.7570000+00:00\",\"Details\":null,\"AppID\":\"THDT\",\"CurrencyCode\":\"CNY\",\"Type\":\"Game\",\"TransactionOCode\":\"xey6kurg58ufq\",\"StartBalance\":120.6,\"EndBalance\":121.8}]},\"nextId\":\"eyIyMDIzMDUyOTE0NTEiOnsiQ3JlYXRlZFRpbWUiOiI0NmJkZTU0MS1mZGVkLTExZWQtYjM4ZS1iZmVlZjQzMjQ0OWIifX0=\",\"games\":[{\"GameCode\":\"naagsa5ycfugq\",\"GameName\":\"Ancient Egypt\",\"GameType\":\"Slot\"}]}";

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url)
				.WithQueryString("appid", appId)
				.WithQueryString("signature", signature)
				.WithQueryString("timeZone", timeZone)
				.WithContent(await content.ReadAsStringAsync())
				.Respond(HttpStatusCode.OK, new StringContent(respondContent));

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			fakeLog,
			fakeSystemClock,
			fakeSignatureUtensil,
			fakeHttpClient);

		var actual = await client.RetrieveTransactionsByMinutesAsync(
			apiUrl: apiUrl,
			appId: appId,
			secretKey: secretKey,
			startDate: startDate,
			endDate: endDate,
			nextId: nextId);

		Assert.NotNull(actual);
		Assert.Equal("eyIyMDIzMDUyOTE0NTEiOnsiQ3JlYXRlZFRpbWUiOiI0NmJkZTU0MS1mZGVkLTExZWQtYjM4ZS1iZmVlZjQzMjQ0OWIifX0=", actual.NextId);
		Assert.Equal("qpp5ra4k5ax4a", actual.Transactions.Single().OCode);
		Assert.Equal("4UCMQPLXB029", actual.Transactions.Single().Username);
		Assert.Equal("naagsa5ycfugq", actual.Transactions.Single().GameCode);
		Assert.Equal("Main Scroll", actual.Transactions.Single().Description);
		Assert.Equal(1.2m, actual.Transactions.Single().Amount);
		Assert.Equal(2.4m, actual.Transactions.Single().Result);
		Assert.Equal(DateTimeOffset.Parse("2023-05-29T06:51:31.7570000+00:00"), actual.Transactions.Single().Time);
	}
}
