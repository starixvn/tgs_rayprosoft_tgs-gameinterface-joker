using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using OpenTelemetry.Exporter;
using Tgs.GameInterface.Joker.Core;

namespace Tgs.GameInterface.Joker.IntegrationTest;

internal class GameInterfaceCRSportsApplication(Action<IWebHostBuilder>? webHostConfigure = null) : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		_ = builder
			.ConfigureAppConfiguration(c => c.Sources.Clear())
			.ConfigureServices(services => _ = services.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
				sp => new ConfigureNamedOptions<OtlpExporterOptions>(
					Options.DefaultName,
					exporterOptions => exporterOptions.TimeoutMilliseconds = 0))
			.AddSingleton(Substitute.For<ISignatureUtensil>())
			.AddSingleton(Substitute.For<IGameApiClient>()));
		webHostConfigure?.Invoke(builder);
	}
}
