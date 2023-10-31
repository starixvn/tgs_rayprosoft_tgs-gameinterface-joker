using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.GameInterface.Joker.Core.Models;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class PlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new PlayerBalanceQuery(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            PlayerCode: "4ucmqplxb029");

        var result = new GetCreditResult(
            IsPlayerExist: true,
            Credit: 1100m,
            OutstandingCredit: 0m);
        _ = fakeGameApiClient
             .GetCreditAsync(
                 apiUrl: Arg.Is(query.ApiUrl),
                 appId: Arg.Is(query.AppId),
                 secretKey: Arg.Is(query.SecretKey),
                 username: Arg.Is(query.PlayerCode),
                 cancellationToken: Arg.Any<CancellationToken>())
             .Returns(result);

        var client = new PlayerBalanceGetService(
            fakeGameApiClient);

        var actual = await client.GetAsync(query);

        Assert.NotNull(actual);
        Assert.True(actual.IsPlayerExist);
    }
}
