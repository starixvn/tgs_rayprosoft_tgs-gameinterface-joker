using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class GameTokenGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new GameTokenQuery(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            PlayerCode: "4ucmqplxb029");

        var client = new GameTokenGetService(
            fakeGameApiClient);

        _ = await client
            .GetAsync(
               query)
           .ConfigureAwait(false);

        _ = await fakeGameApiClient
            .Received(1)
            .PlayGameAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                appId: Arg.Is(query.AppId),
                secretKey: Arg.Is(query.SecretKey),
                username: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
