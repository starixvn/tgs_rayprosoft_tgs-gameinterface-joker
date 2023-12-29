using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.GameInterface.Joker.Core.Models;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class GameRecordQueryUnitTest
{
    [Fact]
    public async Task Normal_Process_Settled_Lose()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new GameRecordQuery(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            StartTime: DateTimeOffset.Parse("2023-05-29T06:50:00+00:00"),
            EndTime: DateTimeOffset.Parse("2023-05-29T07:00:00+00:00"),
            NextId: string.Empty);

        var result = new RetrieveTransactionsResult(
            NextId: "eyIyMDIzMDUyOTE0NTEiOnsiQ3JlYXRlZFRpbWUiOiI0NmJkZTU0MS1mZGVkLTExZWQtYjM4ZS1iZmVlZjQzMjQ0OWIifX0=",
            Transactions: new TransactionResult[]
            {
                new(
                    OCode: "qpp5fiarswx4a",
                    Username: "4UCMQPLXB029",
                    GameCode: "naagsa5ycfugq",
                    Description: "Main Scroll",
                    Amount: 0.8m,
                    Result: 0m,
                    Time: DateTimeOffset.Parse("2023-05-29T06:51:42.2570000+00:00"),
                    AdditionalData: "{\"OCode\":\"qpp5fiarswx4a\",\"Username\":\"4UCMQPLXB029\",\"GameCode\":\"naagsa5ycfugq\",\"Description\":\"Main Scroll\",\"RoundID\":\"qpp5fiarswx4a\",\"Amount\":0.8,\"FreeAmount\":0.0,\"Result\":0.0,\"Time\":\"2023-05-29T06:51:42.2570000+00:00\",\"Details\":null,\"AppID\":\"THDT\",\"CurrencyCode\":\"CNY\",\"Type\":\"Game\",\"TransactionOCode\":\"xey6kurg58ufq\",\"StartBalance\":121.8,\"EndBalance\":121.0}"
                    )
             }
           );
        _ = fakeGameApiClient
            .RetrieveTransactionsByMinutesAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                appId: Arg.Is(query.AppId),
                secretKey: Arg.Is(query.SecretKey),
                startDate: Arg.Is(query.StartTime),
                endDate: Arg.Is(query.EndTime),
                nextId: Arg.Is(query.NextId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var client = new GameRecordQueryService(
            fakeGameApiClient);

        var actual = await client.GetAsync(query);

        Assert.NotNull(actual);
        Assert.True(actual.GameRecordInfos.Any());
        Assert.Equal(result.NextId, actual.NextId);
        Assert.Equal(result.Transactions.Single().OCode.ToString(), actual.GameRecordInfos.Single().ExternalRoundId);
        Assert.Equal(result.Transactions.Single().GameCode.ToString(), actual.GameRecordInfos.Single().ExternalGameId);
        Assert.Equal(result.Transactions.Single().Username.ToLower(), actual.GameRecordInfos.Single().PlayerCode);
        Assert.Equal(result.Transactions.Single().Time, actual.GameRecordInfos.Single().ExternalCreatedOn);
        Assert.Equal(result.Transactions.Single().Time, actual.GameRecordInfos.Single().ExternalClosedOn);
        Assert.Equal(result.Transactions.Single().Amount, actual.GameRecordInfos.Single().Bet);
        Assert.Equal(result.Transactions.Single().Result, actual.GameRecordInfos.Single().Settle);
        Assert.Equal(result.Transactions.Single().Result - result.Transactions.Single().Amount, actual.GameRecordInfos.Single().WinLoss);
        Assert.NotNull(actual.GameRecordInfos.Single().AdditionalData);
    }

    [Fact]
    public async Task Normal_Process_Settled_Win()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new GameRecordQuery(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            StartTime: DateTimeOffset.Parse("2023-05-29T06:50:00+00:00"),
            EndTime: DateTimeOffset.Parse("2023-05-29T07:00:00+00:00"),
            NextId: string.Empty);

        var result = new RetrieveTransactionsResult(
            NextId: "eyIyMDIzMDUyOTE0NTEiOnsiQ3JlYXRlZFRpbWUiOiI0NmJkZTU0MS1mZGVkLTExZWQtYjM4ZS1iZmVlZjQzMjQ0OWIifX0=",
            Transactions: new TransactionResult[]
            {
                new(
                    OCode: "qpp5ra4k5ax4a",
                    Username: "4UCMQPLXB029",
                    GameCode: "naagsa5ycfugq",
                    Description: "Main Scroll",
                    Amount: 1.2m,
                    Result: 2.4m,
                    Time: DateTimeOffset.Parse("2023-05-29T06:51:31.7570000+00:00"),
                    AdditionalData: "{\"OCode\":\"qpp5ra4k5ax4a\",\"Username\":\"4UCMQPLXB029\",\"GameCode\":\"naagsa5ycfugq\",\"Description\":\"Main Scroll\",\"RoundID\":\"qpp5ra4k5ax4a\",\"Amount\":1.2,\"FreeAmount\":0.0,\"Result\":2.4,\"Time\":\"2023-05-29T06:51:31.7570000+00:00\",\"Details\":null,\"AppID\":\"THDT\",\"CurrencyCode\":\"CNY\",\"Type\":\"Game\",\"TransactionOCode\":\"xey6kurg58ufq\",\"StartBalance\":120.6,\"EndBalance\":121.8}"
                    )
             }
           );
        _ = fakeGameApiClient
            .RetrieveTransactionsByMinutesAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                appId: Arg.Is(query.AppId),
                secretKey: Arg.Is(query.SecretKey),
                startDate: Arg.Is(query.StartTime),
                endDate: Arg.Is(query.EndTime),
                nextId: Arg.Is(query.NextId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var client = new GameRecordQueryService(
            fakeGameApiClient);

        var actual = await client.GetAsync(query);

        Assert.NotNull(actual);

        Assert.True(actual.GameRecordInfos.Any());
        Assert.Equal(result.NextId, actual.NextId);
        Assert.Equal(result.Transactions.Single().OCode.ToString(), actual.GameRecordInfos.Single().ExternalRoundId);
        Assert.Equal(result.Transactions.Single().GameCode.ToString(), actual.GameRecordInfos.Single().ExternalGameId);
        Assert.Equal(result.Transactions.Single().Username.ToLower(), actual.GameRecordInfos.Single().PlayerCode);
        Assert.Equal(result.Transactions.Single().Time, actual.GameRecordInfos.Single().ExternalCreatedOn);
        Assert.Equal(result.Transactions.Single().Time, actual.GameRecordInfos.Single().ExternalClosedOn);
        Assert.Equal(result.Transactions.Single().Amount, actual.GameRecordInfos.Single().Bet);
        Assert.Equal(result.Transactions.Single().Result, actual.GameRecordInfos.Single().Settle);
        Assert.Equal(result.Transactions.Single().Result - result.Transactions.Single().Amount, actual.GameRecordInfos.Single().WinLoss);
        Assert.NotNull(actual.GameRecordInfos.Single().AdditionalData);
    }
}
