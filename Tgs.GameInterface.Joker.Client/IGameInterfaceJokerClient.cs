using Tgs.GameInterface.Joker.Protos;

namespace Tgs.GameInterface.Joker.Client;

public interface IGameInterfaceJokerClient
{
	ValueTask<string> CheckingServiceAsync(
		string machineName,
		CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundInAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		string transactionId,
		decimal amount,
		CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundOutAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		string transactionId,
		decimal amount,
		CancellationToken cancellationToken = default);

	ValueTask<bool> FundStatusAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string transactionId,
		CancellationToken cancellationToken = default);

	ValueTask<GameRecordReply> GameRecordAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		string nextId,
		CancellationToken cancellationToken = default);

	ValueTask<string> GameRecordDetailsAsync(
			Uri apiUrl,
		string appId,
		string secretKey,
		string externalRoundId,
		string language,
		CancellationToken cancellationToken = default);

	ValueTask<string> GameTokenAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default);

	ValueTask PlayerCreateAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(
		Uri apiUrl,
		string appId,
		string secretKey,
		string playerCode,
		CancellationToken cancellationToken = default);
}
