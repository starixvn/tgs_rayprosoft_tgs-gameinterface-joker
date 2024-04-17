using System.Diagnostics;

namespace Tgs.GameInterface.Joker;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } =
		new ActivitySource($"Tgs.GameInterface.Joker.DiagnosticsInterceptor");
}
