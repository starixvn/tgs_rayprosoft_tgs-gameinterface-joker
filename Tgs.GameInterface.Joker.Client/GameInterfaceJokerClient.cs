using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.Joker.Protos;

namespace Tgs.GameInterface.Joker.Client;

internal class GameInterfaceJokerClient(
	GameInterfaceJoker.GameInterfaceJokerClient client) : IGameInterfaceJokerClient
{
	public async ValueTask<string> CheckingServiceAsync(
		string machineName,
		CancellationToken cancellationToken = default)
	{
		var request = new CheckingServiceRequest
		{
			MachineName = machineName,
		};

		var reply = await client
			.CheckingServiceAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.MachineName;
	}

	public async ValueTask<FundReply> FundInAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		string transactionId,
		decimal amount,
		CancellationToken cancellationToken = default)
	{
		var request = new FundRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			PlayerCode = playerCode,
			TransactionId = transactionId,
			Amount = amount
		};

		var reply = await client
			.FundInAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply;
	}

	public async ValueTask<FundReply> FundOutAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		string transactionId,
		decimal amount,
		CancellationToken cancellationToken = default)
	{
		var request = new FundRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			PlayerCode = playerCode,
			TransactionId = transactionId,
			Amount = amount
		};

		var reply = await client
			.FundOutAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply;
	}

	public async ValueTask<bool> FundStatusAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string transactionId,
		CancellationToken cancellationToken = default)
	{
		var request = new FundStatusRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			TransactionId = transactionId
		};

		var reply = await client
			.FundStatusAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.State;
	}

	public async ValueTask<GameRecordReply> GameRecordAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		string nextId,
		CancellationToken cancellationToken = default)
	{
		var request = new GameRecordRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp(),
			NextId = nextId
		};

		return await client
			.GameRecordAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<string> GameRecordDetailsAsync(
			Uri apiUrl,
		string appId,
		string secretKey,
		string externalRoundId,
		string language,
		CancellationToken cancellationToken = default)
	{
		var request = new GameRecordDetailsRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			ExternalRoundId = externalRoundId,
			Language = language,
		};

		var reply = await client
			.GameRecordDetailsAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.Url;
	}

	public async ValueTask<string> GameTokenAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var request = new GameTokenRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			PlayerCode = playerCode
		};

		var reply = await client
			.GameTokenAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.Token;
	}

	public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var request = new PlayerBalanceRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			PlayerCode = playerCode
		};

		return await client
			.PlayerBalanceAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask PlayerCreateAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var request = new PlayerCreateRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			PlayerCode = playerCode
		};

		_ = await client
			.PlayerCreateAsync(
			   request: request,
			   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}

	public async ValueTask PlayerLogoutAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var request = new PlayerLogoutRequest
		{
			ApiUrl = apiUrl.ToString(),
			AppId = appId,
			SecretKey = secretKey,
			PlayerCode = playerCode
		};

		_ = await client
			.PlayerLogoutAsync(
			   request: request,
			   cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
