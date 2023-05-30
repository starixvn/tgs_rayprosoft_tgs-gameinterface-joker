using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class PlayerCreateCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var command = new PlayerCreateCommand(
            ApiUrl: new Uri("https://w.api788.net/"),
            AppId: "THDT",
            SecretKey: "s5gawreoto5e1",
            PlayerCode: "4ucmqplxb029");

        var client = new PlayerCreateCommandService(
            fakeGameApiClient);

        await client
           .ExecuteAsync(
              command)
          .ConfigureAwait(false);

        await fakeGameApiClient
           .Received(1)
        .CreateUserAsync(
                 apiUrl: Arg.Is(command.ApiUrl),
                 appId: Arg.Is(command.AppId),
                 secretKey: Arg.Is(command.SecretKey),
                 username: Arg.Is(command.PlayerCode),
               cancellationToken: Arg.Any<CancellationToken>())
           .ConfigureAwait(false);
    }
}
