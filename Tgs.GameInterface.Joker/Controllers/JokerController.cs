using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.GameInterface.Joker.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.Controllers;

[Route("[Controller]")]
[ApiController]
public class JokerController : ControllerBase
{
	/// <summary>
	/// Funds the in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "playerCode": "4ucmqplxb029",
	///             "transactionId": "D924DB2E9E409C8AAAD6341955E3FEBE",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("fund/in", Name = "FundIn")]
	public async ValueTask<FundResultViewModel> FundInAsync(
		[FromServices] IGetService<FundInQuery, FundResult> service,
		FundViewModel source)
	{
		var query = new FundInQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			PlayerCode: source.PlayerCode,
			TransactionId: source.TransactionId,
			Amount: source.Amount);

		var result = await service
			.GetAsync(
				query: query,
				cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new FundResultViewModel(
			RequestId: result.RequestId,
			Res: result.Res);
	}

	/// <summary>
	/// Funds the out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "playerCode": "4ucmqplxb029",
	///             "transactionId": "E8E7425C214D59B22952A8E18FA9B7A4",
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("fund/out", Name = "FundOut")]
	public async ValueTask<FundResultViewModel> FundOutAsync(
		[FromServices] IGetService<FundOutQuery, FundResult> service,
		FundViewModel source)
	{
		var query = new FundOutQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			PlayerCode: source.PlayerCode,
			TransactionId: source.TransactionId,
			Amount: source.Amount);

		var result = await service
			.GetAsync(
				query: query,
				cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new FundResultViewModel(
			RequestId: result.RequestId,
			Res: result.Res);
	}

	/// <summary>
	/// Funds the status asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "transactionId": "E8E7425C214D59B22952A8E18FA9B7A4"
	///         }
	/// </remarks>
	[HttpPost("fund/Status", Name = "FundStatus")]
	public ValueTask<bool> FundStatusAsync(
		[FromServices] IGetService<FundStatusQuery, bool> service,
		FundStatusViewModel source)
	{
		var query = new FundStatusQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			TransactionId: source.TransactionId);

		return service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Games the record asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "startTime": "2023-05-29T06:50:00+00:00",
	///             "endTime": "2023-05-29T07:00:00+00:00",
	///             "nextId": ""
	///         }
	/// </remarks>
	[HttpPost("game/record")]
	public async ValueTask<GameRecordResultViewModel> GameRecordAsync(
			[FromServices] IGetService<GameRecordQuery, GameRecordResult> service,
			GameRecordViewModel source)
	{
		var query = new GameRecordQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			StartTime: source.StartTime,
			EndTime: source.EndTime,
			NextId: source.NextId);

		var result = await service
			.GetAsync(
				query: query,
				cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameRecordResultViewModel(
			NextId: result.NextId,
			GameRecordInfos: result.GameRecordInfos.Select(x => new GameRecordInfoViewModel(
				ExternalRoundId: x.ExternalRoundId,
				ExternalGameId: x.ExternalGameId,
				PlayerCode: x.PlayerCode,
				ExternalCreatedOn: x.ExternalCreatedOn,
				ExternalClosedOn: x.ExternalClosedOn,
				Bet: x.Bet,
				Settle: x.Settle,
				WinLoss: x.WinLoss,
				AdditionalData: x.AdditionalData)));
	}

	/// <summary>
	/// Games the record details asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "externalRoundId": "qppiorex5ax4a",
	///             "language": "cn"
	///         }
	/// </remarks>
	[HttpPost("game/record/details")]
	public ValueTask<string> GameRecordDetailsAsync(
			[FromServices] IGetService<GameRecordDetailsQuery, string> service,
			GameRecordDetailsViewModel source)
	{
		var query = new GameRecordDetailsQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			ExternalRoundId: source.ExternalRoundId,
			Language: source.Language);

		return service
			.GetAsync(
				query: query,
				cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Games the token asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("game-token")]
	public ValueTask<string> GameTokenAsync(
		[FromServices] IGetService<GameTokenQuery, string> service,
		GameTokenQueryViewModel source)
	{
		var query = new GameTokenQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			PlayerCode: source.PlayerCode);

		return service
		 .GetAsync(
			 query: query,
			 cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("player/balance", Name = "PlayerBalance")]
	public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		PlayerBalanceViewModel source)
	{
		var query = new PlayerBalanceQuery(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			PlayerCode: source.PlayerCode);

		var result = await service
				.GetAsync(
					query: query,
					cancellationToken: HttpContext.RequestAborted)
				.ConfigureAwait(false);

		return new PlayerBalanceResultViewModel(
			result.IsPlayerExist,
			result.Balance);
	}

	/// <summary>
	/// Players the create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("player/create", Name = "PlayerCreate")]
	public ValueTask PlayerCreateAsync(
		[FromServices] ICommandService<PlayerCreateCommand> service,
		PlayerCreateViewModel source)
	{
		var command = new PlayerCreateCommand(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			PlayerCode: source.PlayerCode);

		return service.ExecuteAsync(
				command: command,
				cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Players the kick asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://w.api788.net/",
	///             "appId": "THDT",
	///             "secretKey": "s5gawreoto5e1",
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("player/logout", Name = "PlayerLogout")]
	public ValueTask PlayerKickAsync(
		[FromServices] ICommandService<PlayerLogoutCommand> service,
		PlayerLogoutViewModel source)
	{
		var command = new PlayerLogoutCommand(
			ApiUrl: source.ApiUrl,
			AppId: source.AppId,
			SecretKey: source.SecretKey,
			PlayerCode: source.PlayerCode);

		return service
			.ExecuteAsync(
				command: command,
				cancellationToken: HttpContext.RequestAborted);
	}
}
