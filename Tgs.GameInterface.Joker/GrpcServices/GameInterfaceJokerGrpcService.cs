using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.Joker.Core.ApplicationServices;
using Tgs.GameInterface.Joker.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.Joker.GrpcServices;

public class GameInterfaceJokerGrpcService(
    ILogger<GameInterfaceJokerGrpcService> logger,
    IGetService<FundInQuery, FundResult> fundInService,
    IGetService<FundOutQuery, FundResult> fundOutService,
    IGetService<FundStatusQuery, bool> fundStatusService,
    IGetService<GameRecordDetailsQuery, string> gameRecordDetailsService,
    IGetService<GameRecordQuery, GameRecordResult> gameRecordService,
    IGetService<GameTokenQuery, string> gameTokenService,
    IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
    ICommandService<PlayerCreateCommand> playerCreateService,
    ICommandService<PlayerLogoutCommand> PlayerLogoutService) : GameInterfaceJoker.GameInterfaceJokerBase
{
    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

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

        var result = await fundInService
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

        var result = await fundOutService
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

        var result = await fundStatusService
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

        var result = await gameRecordService
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

        var result = await gameRecordDetailsService
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

        var result = await gameTokenService
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

        var result = await playerBalanceService
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

        await playerCreateService
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

        await PlayerLogoutService
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
