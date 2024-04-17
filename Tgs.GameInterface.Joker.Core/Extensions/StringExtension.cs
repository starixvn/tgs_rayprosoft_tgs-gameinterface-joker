namespace Tgs.GameInterface.Joker.Core.Extensions;

public static class StringExtension
{
	public static string NoLongerThan(this string str, int chars)
		=> string.IsNullOrEmpty(str) || str.Length <= chars ? str : str[..chars];
}
