using System.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.Joker.Core.ApplicationServices;
public record DomainRegisterWhitelistQuery
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
	[Required]
	[DefaultValue("[\"dev-t1.rpgp.cc\",\"stg-t1.rpgp.cc\"]")]
	public IEnumerable<string> Domains { get; init; } = default!;
}
