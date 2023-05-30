using System.Text.Json.Nodes;

namespace Tgs.GameInterface.Joker.Core;

public interface ISignatureUtensil
{
    public string GenerateSignature(JsonNode source, string secretKey);
}
