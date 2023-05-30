using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class GameRecordDetailsQueryServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new GameRecordDetailsQuery(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            ExternalRoundId: "qppiorex5ax4a",
            Language: "zh");

        var client = new GameRecordDetailsQueryService(
            fakeGameApiClient);

        _ = await client
           .GetAsync(
              query)
          .ConfigureAwait(false);

        _ = await fakeGameApiClient
           .Received(1)
           .RetrieveHistoryUrlAsync(
                 apiUrl: Arg.Is(query.ApiUrl),
                 appId: Arg.Is(query.AppId),
                 secretKey: Arg.Is(query.SecretKey),
                 oCode: Arg.Is(query.ExternalRoundId),
                 gameType: "Game",
                 language: Arg.Is(query.Language),
                 cancellationToken: Arg.Any<CancellationToken>())
           .ConfigureAwait(false);
    }
}
