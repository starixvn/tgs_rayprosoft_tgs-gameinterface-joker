using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Joker.Protos;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterfacePinnacleClient(
        this IServiceCollection services,
        Action<GameInterfaceJokerClientOptions, IServiceProvider> configure)
        => services
            .AddOptions<GameInterfaceJokerClientOptions>()
            .Configure(configure)
            .Services
            .AddGrpcClient<GameInterfaceJoker.GameInterfaceJokerClient>((sp, configureClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameInterfaceJokerClientOptions>>().Value;
                configureClient.Address = options.BaseUrl;
            })
            .ConfigureChannel((sp, configureChannel) =>
            {
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
                var defaultMethodConfig = new MethodConfig
                {
                    Names = { MethodName.Default },
                    RetryPolicy = new RetryPolicy
                    {
                        //呼叫嘗試次數上限，包括原始嘗試。
                        //此值會受到限制 GrpcChannelOptions.MaxRetryAttempts ，預設為 5。
                        //值是必要的，而且必須大於 1。
                        MaxAttempts = 2,
                        //重試嘗試之間的初始輪詢延遲。
                        //介於 0 與目前輪詢之間的隨機延遲會決定下一次重試嘗試的時間。
                        //每次嘗試之後，目前的輪詢會乘以 BackoffMultiplier 。
                        //值是必要的，而且必須大於零。
                        InitialBackoff = TimeSpan.FromSeconds(1),
                        //最大輪詢會對指數輪詢成長設定上限。
                        //值是必要的，而且必須大於零。
                        MaxBackoff = TimeSpan.FromSeconds(5),
                        //每次重試嘗試之後，輪詢都會乘以此值，並在乘數大於 1 時以指數方式增加。
                        //值是必要的，而且必須大於零。
                        BackoffMultiplier = 1.5,
                        //狀態碼的集合。
                        //系統會自動重試失敗且狀態相符的 gRPC 呼叫。
                        //如需狀態碼的詳細資訊，請參閱 狀態碼及其在 gRPC 中使用。 至少需要一個可重試的狀態碼。
                        RetryableStatusCodes = { StatusCode.Unavailable }
                    }
                };
                configureChannel.HttpHandler = handler;
                configureChannel.Credentials = ChannelCredentials.Insecure;
                configureChannel.ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } };
            })
            .AddInterceptor<LoggingInterceptor>()
            .Services
            //負載平衡時，效能很重要。 藉由快取位址，解析位址的延遲會從 gRPC 呼叫中排除。 進行第一個 gRPC 呼叫時，將會叫用解析程式，而後續的呼叫會使用快取。
            .AddSingleton<ResolverFactory>(sp => new DnsResolverFactory(refreshInterval: TimeSpan.FromSeconds(30)))
            .AddSingleton<DiagnosticsInterceptorOptions>()
            .AddSingleton<LoggingInterceptor>()
            .AddTransient<IGameInterfaceJokerClient, GameInterfaceJokerClient>();
}
