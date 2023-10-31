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

public class GetCreditUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var apiUrl = new Uri("https://w.api788.net/");
        var appId = "THDT";
        var secretKey = "s5gawreoto5e1";
        var username = "4ucmqplxb029";

        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeSignatureUtensil = Substitute.For<ISignatureUtensil>();

        var currentTime = DateTimeOffset.Parse("2023-05-29T11:24:18+08:00");
        _ = fakeSystemClock
            .CurrentTime
            .Returns(currentTime);

        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "GC");
                writer.WriteString("Username", username);
                writer.WriteNumber("Timestamp", currentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = "SCGv1Twtmv8L0FlmTnXfb7Gj+os=";
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

        var respondContent = "{\"Username\":\"4UCMQPLXB029\",\"Credit\":0.0,\"OutstandingCredit\":0.0,\"FreeCredit\":0.0,\"OutstandingFreeCredit\":0.0}";

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

        var actual = await client.GetCreditAsync(
            apiUrl: apiUrl,
            appId: appId,
            secretKey: secretKey,
            username: username);

        Assert.NotNull(actual);
        Assert.True(actual.IsPlayerExist);
        Assert.Equal(expected: 0m, actual.Credit);
        Assert.Equal(expected: 0m, actual.OutstandingCredit);
    }

    [Fact]
    public async void Was_Not_Found()
    {
        var apiUrl = new Uri("https://w.api788.net/");
        var appId = "THDT";
        var secretKey = "s5gawreoto5e1";
        var username = "4ucmqplxb029";

        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeSignatureUtensil = Substitute.For<ISignatureUtensil>();

        var currentTime = DateTimeOffset.Parse("2023-05-29T11:24:18+08:00");
        _ = fakeSystemClock
            .CurrentTime
            .Returns(currentTime);

        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "GC");
                writer.WriteString("Username", username);
                writer.WriteNumber("Timestamp", currentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = "SCGv1Twtmv8L0FlmTnXfb7Gj+os=";
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

        var respondContent = "{\"Message\": \"THDT.4ucmqplxb029 was not found\"}";

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url)
                .WithQueryString("appid", appId)
                .WithQueryString("signature", signature)
                .WithContent(await content.ReadAsStringAsync())
                .Respond(HttpStatusCode.NotFound, new StringContent(respondContent));

        var fakeLog = NullLogger<GameApiClient>.Instance;
        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            fakeLog,
            fakeSystemClock,
            fakeSignatureUtensil,
            fakeHttpClient);

        var actual = await client.GetCreditAsync(
            apiUrl: apiUrl,
            appId: appId,
            secretKey: secretKey,
            username: username);

        Assert.NotNull(actual);
        Assert.False(actual.IsPlayerExist);
        Assert.Equal(expected: 0m, actual.Credit);
        Assert.Equal(expected: 0m, actual.OutstandingCredit);
    }
}
