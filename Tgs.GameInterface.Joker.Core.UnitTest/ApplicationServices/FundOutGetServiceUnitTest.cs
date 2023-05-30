using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.GameInterface.Joker.Core.Models;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class FundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new FundOutQuery(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            PlayerCode: "4ucmqplxb029",
            TransactionId: "01886633af3f0000000107e78f2b8c75",
            Amount: 123m);

        var result = new TransferCreditResult(
            RequestId: "01886638f0830000000107e78f2b8c75",
            Res: "{\"RequestID\":\"01886633af3f0000000107e78f2b8c75\",\"Username\":\"4UCMQPLXB029\",\"Time\":\"2023-05-29T14:30:38.3257012\\u002B08:00\",\"Credit\":123.0,\"BeforeCredit\":0.0,\"OutstandingCredit\":0.0}");
        _ = fakeGameApiClient
             .DepositAsync(
                 apiUrl: Arg.Is(query.ApiUrl),
                 appId: Arg.Is(query.AppId),
                 secretKey: Arg.Is(query.SecretKey),
                 username: Arg.Is(query.PlayerCode),
                 requestId: Arg.Is(query.TransactionId),
                 amount: Arg.Is(query.Amount),
                 cancellationToken: Arg.Any<CancellationToken>())
             .Returns(result);

        var client = new FundOutGetService(
            fakeGameApiClient);

        var actual = await client
            .GetAsync(
               query)
           .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(result.RequestId, actual.RequestId);
        Assert.Equal(result.Res, actual.Res);
    }
}
