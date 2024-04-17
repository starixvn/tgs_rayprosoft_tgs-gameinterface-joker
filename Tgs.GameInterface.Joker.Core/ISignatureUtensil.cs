namespace Tgs.GameInterface.Joker.Core;

public interface ISignatureUtensil
{
	public string GenerateSignature(System.Text.Json.Nodes.JsonNode source, string secretKey);
}
