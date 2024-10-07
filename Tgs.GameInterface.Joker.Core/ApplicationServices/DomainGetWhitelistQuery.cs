using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;
public record DomainGetWhitelistQuery
{
	[Required]
	[DefaultValue("https://w.api788.net/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue("THDT")]
	public string AppId { get; init; } = default!;
	[Required]
	[DefaultValue("s5gawreoto5e1")]
	public string SecretKey { get; init; } = default!;
}
