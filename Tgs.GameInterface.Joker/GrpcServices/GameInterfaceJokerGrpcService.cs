using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.GameInterface.Joker.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.GrpcServices;

public class GameInterfaceJokerGrpcService : GameInterfaceJoker.GameInterfaceJokerBase
{
    private readonly IGetService<FundInQuery, FundResult> _fundInService;
    private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
    private readonly IGetService<FundStatusQuery, bool> _fundStatusService;
    private readonly IGetService<GameRecordDetailsQuery, string> _gameRecordDetailsService;
    private readonly IGetService<GameRecordQuery, GameRecordResult> _gameRecordService;
    private readonly IGetService<GameTokenQuery, string> _gameTokenService;
    private readonly ILogger<GameInterfaceJokerGrpcService> _logger;
    private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
    private readonly ICommandService<PlayerCreateCommand> _playerCreateService;
    private readonly ICommandService<PlayerLogoutCommand> _playerLogoutService;

    public GameInterfaceJokerGrpcService(
        ILogger<GameInterfaceJokerGrpcService> logger,
        IGetService<FundInQuery, FundResult> fundInService,
        IGetService<FundOutQuery, FundResult> fundOutService,
        IGetService<FundStatusQuery, bool> fundStatusService,
        IGetService<GameRecordDetailsQuery, string> gameRecordDetailsService,
        IGetService<GameRecordQuery, GameRecordResult> gameRecordService,
        IGetService<GameTokenQuery, string> gameTokenService,
        IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
        ICommandService<PlayerCreateCommand> playerCreateService,
        ICommandService<PlayerLogoutCommand> PlayerLogoutService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        _fundStatusService = fundStatusService ?? throw new ArgumentNullException(nameof(fundStatusService));
        _gameRecordDetailsService = gameRecordDetailsService ?? throw new ArgumentNullException(nameof(gameRecordDetailsService));
        _gameRecordService = gameRecordService ?? throw new ArgumentNullException(nameof(gameRecordService));
        _gameTokenService = gameTokenService ?? throw new ArgumentNullException(nameof(gameTokenService));
        _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
        _playerCreateService = playerCreateService ?? throw new ArgumentNullException(nameof(playerCreateService));
        _playerLogoutService = PlayerLogoutService ?? throw new ArgumentNullException(nameof(PlayerLogoutService));
    }

    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        _logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

        return Task.FromResult(new CheckingServiceReply
        {
            MachineName = Environment.MachineName
        });
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            PlayerCode: request.PlayerCode,
            TransactionId: request.TransactionId,
            Amount: request.Amount);

        var result = await _fundInService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            RequestId = result.RequestId,
            Res = result.Res
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            PlayerCode: request.PlayerCode,
            TransactionId: request.TransactionId,
            Amount: request.Amount);

        var result = await _fundOutService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            RequestId = result.RequestId,
            Res = result.Res
        };
    }

    public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            TransactionId: request.TransactionId);

        var result = await _fundStatusService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = result
        };
    }

    public override async Task<GameRecordReply> GameRecord(GameRecordRequest request, ServerCallContext context)
    {
        var query = new GameRecordQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            StartTime: request.StartTime.ToDateTimeOffset(),
            EndTime: request.EndTime.ToDateTimeOffset(),
            NextId: request.NextId);

        var result = await _gameRecordService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        var reply = new GameRecordReply
        {
            NextId = result.NextId,
        };

        reply.Items.AddRange(result.GameRecordInfos.Select(x => new Protos.GameRecordInfo
        {
            ExternalRoundId = x.ExternalRoundId,
            ExternalGameId = x.ExternalGameId,
            PlayerCode = x.PlayerCode,
            ExternalCreatedOn = x.ExternalCreatedOn.ToTimestamp(),
            ExternalClosedOn = x.ExternalClosedOn.ToTimestamp(),
            Bet = x.Bet,
            Settle = x.Settle,
            WinLoss = x.WinLoss,
            AdditionalData = x.AdditionalData
        }) ?? Enumerable.Empty<Protos.GameRecordInfo>());

        return reply;
    }

    public override async Task<GameRecordDetailsReply> GameRecordDetails(GameRecordDetailsRequest request, ServerCallContext context)
    {
        var query = new GameRecordDetailsQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            ExternalRoundId: request.ExternalRoundId,
            Language: request.Language);

        var result = await _gameRecordDetailsService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new GameRecordDetailsReply
        {
            Url = result
        };
    }

    public override async Task<GameTokenReply> GameToken(GameTokenRequest request, ServerCallContext context)
    {
        var query = new GameTokenQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            PlayerCode: request.PlayerCode);

        var result = await _gameTokenService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new GameTokenReply
        {
            Token = result
        };
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            PlayerCode: request.PlayerCode);

        var result = await _playerBalanceService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceReply
        {
            IsPlayerExist = result.IsPlayerExist,
            Balance = result.Balance
        };
    }

    public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            PlayerCode: request.PlayerCode);

        await _playerCreateService
         .ExecuteAsync(
             command: command,
             cancellationToken: context.CancellationToken)
         .ConfigureAwait(false);

        return new BaseReply
        {
            State = true
        };
    }

    public override async Task<BaseReply> PlayerLogout(PlayerLogoutRequest request, ServerCallContext context)
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: new(request.ApiUrl),
            AppId: request.AppId,
            SecretKey: request.SecretKey,
            PlayerCode: request.PlayerCode);

        await _playerLogoutService
         .ExecuteAsync(
             command: command,
             cancellationToken: context.CancellationToken)
         .ConfigureAwait(false);

        return new BaseReply
        {
            State = true
        };
    }
}
