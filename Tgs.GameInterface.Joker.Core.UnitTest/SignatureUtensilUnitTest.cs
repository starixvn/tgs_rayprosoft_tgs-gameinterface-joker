using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Tgs.GameInterface.Joker.Core.UnitTest;

public class SignatureUtensilUnitTest
{
    [Fact]
    public void Normal_Process()
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "GC");
                writer.WriteString("Username", "4ucmqplxb029");
                writer.WriteNumber("Timestamp", DateTimeOffset.Parse("2023-05-29T11:24:18+08:00").ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var utensil = new SignatureUtensil();
        var actual = utensil.GenerateSignature(parameters, "s5gawreoto5e1");

        Assert.Equal("SCGv1Twtmv8L0FlmTnXfb7Gj+os=", actual);
    }

    [Fact]
    public void Normal_Process_GameToken()
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "PLAY");
                writer.WriteString("Username", "4ucmqplxb029");
                writer.WriteNumber("Timestamp", DateTimeOffset.Parse("2023-05-29T12:00:28+08:00").ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var utensil = new SignatureUtensil();
        var actual = utensil.GenerateSignature(parameters, "s5gawreoto5e1");

        Assert.Equal("k8hfbm0t7huqpJjg34wBCKFl7d8=", actual);
    }
}
