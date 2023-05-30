using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Tgs.GameInterface.Joker.Core;

internal class SignatureUtensil : ISignatureUtensil
{
    public string GenerateSignature(JsonNode source, string secretKey)
    {
        var parameters = source.Deserialize<SortedDictionary<string, object>>()!;

        var values = parameters.Select(p => $"{p.Key}={p.Value}");
        var original = string.Join("&", values);

        using var sha = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(original));
        return Convert.ToBase64String(hash);
    }
}
