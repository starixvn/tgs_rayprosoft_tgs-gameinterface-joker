using NSubstitute;
using Tgs.GameInterface.Joker.Core.ApplicationServices;

namespace Tgs.GameInterface.Joker.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new FundStatusQuery(
			ApiUrl: new Uri("https://w.api788.net/"),
			AppId: "THDT",
			SecretKey: "s5gawreoto5e1",
			TransactionId: "01886633af3f0000000107e78f2b8c75"
			);

		_ = fakeGameApiClient
			 .VerifyTransferCreditAsync(
				 apiUrl: Arg.Is(query.ApiUrl),
				 appId: Arg.Is(query.AppId),
				 secretKey: Arg.Is(query.SecretKey),
				 requestId: Arg.Is(query.TransactionId),
				 cancellationToken: Arg.Any<CancellationToken>())
			 .Returns(true);

		var client = new FundStatusGetService(
			fakeGameApiClient);

		var actual = await client.GetAsync(query);

		Assert.True(actual);
	}
}
