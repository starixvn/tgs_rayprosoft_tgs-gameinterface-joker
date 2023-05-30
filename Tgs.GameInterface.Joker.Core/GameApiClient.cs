using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Joker.Core.Extensions;
using Tgs.GameInterface.Joker.Core.Models;

namespace Tgs.GameInterface.Joker.Core;

internal class GameApiClient : IGameApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<GameApiClient> _logger;
    private readonly ISignatureUtensil _signatureUtensil;
    private readonly ISystemClock _systemClock;

    public GameApiClient(
        ILogger<GameApiClient> logger,
        ISystemClock systemClock,
        ISignatureUtensil signatureUtensil,
        HttpClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _signatureUtensil = signatureUtensil ?? throw new ArgumentNullException(nameof(signatureUtensil));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask CreateUserAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "CU");
                writer.WriteString("Username", username);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "CreateUser",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!IsSuccessStatusCode)
        {
            throw new Exception($"{JsonNode!["Message"]}( {(int)HttpStatusCode})");
        }
    }

    public async ValueTask<TransferCreditResult> DepositAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        string requestId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "TC");
                writer.WriteString("Username", username);
                writer.WriteString("RequestID", requestId);
                writer.WriteNumber("Amount", amount);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "Deposit",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return !IsSuccessStatusCode
            ? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
            : new TransferCreditResult(
            JsonNode!["RequestID"]!.GetValue<string>(),
            JsonNode.ToJsonString());
    }

    public async ValueTask<GetCreditResult> GetCreditAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "GC");
                writer.WriteString("Username", username);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "GetCredit",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return !IsSuccessStatusCode
            ? HttpStatusCode != HttpStatusCode.NotFound
                ? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
                : new GetCreditResult(false, 0, 0)
            : new GetCreditResult(
            IsPlayerExist: true,
            Credit: JsonNode!["Credit"]!.GetValue<decimal>(),
            OutstandingCredit: JsonNode!["OutstandingCredit"]!.GetValue<decimal>());
    }

    public async ValueTask<string> PlayGameAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "PLAY");
                writer.WriteString("Username", username);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "PlayGame",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return !IsSuccessStatusCode
            ? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
            : JsonNode!["Token"]!.GetValue<string>();
    }

    public async ValueTask<string> RetrieveHistoryUrlAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string oCode,
        string gameType,
        string language,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "History");
                writer.WriteString("OCode", oCode);
                writer.WriteString("Type", gameType);
                writer.WriteString("Language", language);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "RetrieveHistoryUrl",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return !IsSuccessStatusCode
            ? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
            : JsonNode!["Url"]!.GetValue<string>();
    }

    public async ValueTask<RetrieveTransactionsResult> RetrieveTransactionsByMinutesAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string nextId,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "TSM");
                writer.WriteString("StartDate", startDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
                writer.WriteString("EndDate", endDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
                writer.WriteString("NextId", (string.IsNullOrEmpty(nextId) ? string.Empty : nextId));
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);
        queryStrings.Add("timeZone", "Greenwich Standard Time");

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
                apiUrl: apiUrl,
                queryStrings: queryStrings,
                parameters: parameters,
                logPrefix: "RetrieveTransactionsByMinutes",
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!IsSuccessStatusCode)
        {
            throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})");
        }

        var tempNextId = JsonNode!["nextId"]!.GetValue<string>();

        if (string.IsNullOrEmpty(tempNextId))
        {
            return new RetrieveTransactionsResult(
                string.Empty,
                Enumerable.Empty<TransactionResult>());
        }

        var transactionResults = new List<TransactionResult>();

        var games = JsonNode!["data"]?["Game"]?.AsArray();

        if (games is not null && games.Any())
        {
            transactionResults.AddRange(games
                .Where(x => x is not null)
                .Select(x => x!)
                .AsEnumerable()
                .Select(x => new TransactionResult(
                     OCode: x["OCode"]!.GetValue<string>(),
                     Username: x["Username"]!.GetValue<string>(),
                     GameCode: x["GameCode"]!.GetValue<string>(),
                     Description: x["Description"]!.GetValue<string>(),
                     Amount: x["Amount"]!.GetValue<decimal>(),
                     Result: x["Result"]!.GetValue<decimal>(),
                     Time: x["Time"]!.GetValue<DateTimeOffset>(),
                     AdditionalData: x.ToJsonString())
                ));
        }

        var jackpots = JsonNode["data"]?["Jackpot"]?.AsArray();

        if (jackpots is not null && jackpots.Any())
        {
            transactionResults.AddRange(jackpots
                .Where(x => x is not null)
                .Select(x => x!)
                .AsEnumerable()
                .Select(x => new TransactionResult(
                     OCode: x["OCode"]!.GetValue<string>(),
                     Username: x["Username"]!.GetValue<string>(),
                     GameCode: x["GameCode"]!.GetValue<string>(),
                     Description: x["Description"]!.GetValue<string>(),
                     Amount: x["Amount"]!.GetValue<decimal>(),
                     Result: x["Result"]!.GetValue<decimal>(),
                     Time: x["Time"]!.GetValue<DateTimeOffset>(),
                     AdditionalData: x.ToJsonString())
                ));
        }

        var competition = JsonNode["data"]?["Competition"]?.AsArray();

        if (competition is not null && competition.Any())
        {
            transactionResults.AddRange(competition
                .Where(x => x is not null)
                .Select(x => x!)
                .AsEnumerable()
                .Select(x => new TransactionResult(
                     OCode: x["OCode"]!.GetValue<string>(),
                     Username: x["Username"]!.GetValue<string>(),
                     GameCode: x["GameCode"]!.GetValue<string>(),
                     Description: x["Description"]!.GetValue<string>(),
                     Amount: x["Amount"]!.GetValue<decimal>(),
                     Result: x["Result"]!.GetValue<decimal>(),
                     Time: x["Time"]!.GetValue<DateTimeOffset>(),
                     AdditionalData: x.ToJsonString())
                ));
        }

        return new RetrieveTransactionsResult(
            tempNextId,
           (transactionResults.Any()) ? transactionResults : Enumerable.Empty<TransactionResult>());
    }

    public async ValueTask SignOutUserAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "SO");
                writer.WriteString("Username", username);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
                apiUrl: apiUrl,
                queryStrings: queryStrings,
                parameters: parameters,
                logPrefix: "SignOutUser",
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!IsSuccessStatusCode)
        {
            throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})");
        }
    }

    public async ValueTask<bool> VerifyTransferCreditAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string requestId,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "TCH");
                writer.WriteString("RequestID", requestId);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, _) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "VerifyTransferCredit",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return IsSuccessStatusCode && HttpStatusCode != HttpStatusCode.NotFound;
    }

    public async ValueTask<TransferCreditResult> WithdrawAsync(
        Uri apiUrl,
        string appId,
        string secretKey,
        string username,
        string requestId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        JsonNode parameters;

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Method", "TC");
                writer.WriteString("Username", username);
                writer.WriteString("RequestID", requestId);
                writer.WriteNumber("Amount", -amount);
                writer.WriteNumber("Timestamp", _systemClock.CurrentTime.ToUnixTimeSeconds());
                writer.WriteEndObject();
            }
            parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
        }

        var signature = _signatureUtensil
            .GenerateSignature(
                source: parameters,
                secretKey: secretKey);

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
            apiUrl: apiUrl,
            queryStrings: queryStrings,
            parameters: parameters,
            logPrefix: "Deposit",
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return !IsSuccessStatusCode
            ? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
            : new TransferCreditResult(
            JsonNode!["RequestID"]!.GetValue<string>(),
            JsonNode.ToJsonString());
    }

    private async ValueTask<(bool IsSuccessStatusCode, HttpStatusCode HttpStatusCode, JsonNode? JsonNode)> PostAsync(
        Uri apiUrl,
        NameValueCollection queryStrings,
        JsonNode parameters,
        string logPrefix,
        CancellationToken cancellationToken = default)
    {
        logPrefix = $"{nameof(GameApiClient)}.{logPrefix}".ToLower();

        var sw = Stopwatch.StartNew();

        var url = $"{apiUrl.OriginalString}?{queryStrings}";
        var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
        var httpResponseMessage = await _client
            .PostAsync(
                requestUri: url,
                content: content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);

        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
        {
            responseMessage = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
        }

        var apiLogEvent = new GameApiLogEvent
        {
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = (await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };
        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        return new(httpResponseMessage.IsSuccessStatusCode, httpResponseMessage.StatusCode, (!string.IsNullOrEmpty(responseMessage)) ? JsonNode.Parse(responseMessage) : null);
    }
}
