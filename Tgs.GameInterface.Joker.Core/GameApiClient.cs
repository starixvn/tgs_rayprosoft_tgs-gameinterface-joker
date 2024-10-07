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

internal class GameApiClient(
	ILogger<GameApiClient> logger,
	TimeProvider timeProvider,
	ISignatureUtensil signatureUtensil,
	HttpClient client) : IGameApiClient
{
	public async ValueTask CreateUserAsync(Uri apiUrl, string appId, string secretKey, string username, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "CU");
				writer.WriteString("Username", username);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(CreateUserAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!IsSuccessStatusCode)
		{
			throw new Exception($"{JsonNode!["Message"]}( {(int)HttpStatusCode})");
		}
	}

	public async ValueTask<TransferCreditResult> DepositAsync(Uri apiUrl, string appId, string secretKey, string username, string requestId, decimal amount, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "TC");
				writer.WriteString("Username", username);
				writer.WriteString("RequestID", requestId);
				writer.WriteNumber("Amount", amount);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(DepositAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !IsSuccessStatusCode
			? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
			: new TransferCreditResult(
			JsonNode!["RequestID"]!.GetValue<string>(),
			JsonNode.ToJsonString());
	}

	public async ValueTask<GetCreditResult> GetCreditAsync(Uri apiUrl, string appId, string secretKey, string username, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "GC");
				writer.WriteString("Username", username);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(GetCreditAsync),
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

	public async ValueTask<string> PlayGameAsync(Uri apiUrl, string appId, string secretKey, string username, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "PLAY");
				writer.WriteString("Username", username);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(PlayGameAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !IsSuccessStatusCode
			? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
			: JsonNode!["Token"]!.GetValue<string>();
	}

	public async ValueTask<string> RetrieveHistoryUrlAsync(Uri apiUrl, string appId, string secretKey, string oCode, string gameType, string language, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "History");
				writer.WriteString("OCode", oCode);
				writer.WriteString("Type", gameType);
				writer.WriteString("Language", language);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(RetrieveHistoryUrlAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !IsSuccessStatusCode
			? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
			: JsonNode!["Url"]!.GetValue<string>();
	}

	public async ValueTask<RetrieveTransactionsResult> RetrieveTransactionsByMinutesAsync(Uri apiUrl, string appId, string secretKey, DateTimeOffset startDate, DateTimeOffset endDate, string nextId, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "TSM");
				writer.WriteString("StartDate", startDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("EndDate", endDate.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
				writer.WriteString("NextId", string.IsNullOrEmpty(nextId) ? string.Empty : nextId);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);
		queryStrings.Add("timeZone", "Greenwich Standard Time");

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
				apiUrl: apiUrl,
				queryStrings: queryStrings,
				parameters: parameters,
				logPrefix: nameof(RetrieveTransactionsByMinutesAsync),
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
				[]);
		}

		var transactionResults = new List<TransactionResult>();

		var games = JsonNode!["data"]?["Game"]?.AsArray();

		if (games is not null && games.Count != 0)
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

		if (jackpots is not null && jackpots.Count != 0)
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

		if (competition is not null && competition.Count != 0)
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
		   (transactionResults.Count != 0) ? transactionResults : Enumerable.Empty<TransactionResult>());
	}

	public async ValueTask SignOutUserAsync(Uri apiUrl, string appId, string secretKey, string username, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "SO");
				writer.WriteString("Username", username);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
				apiUrl: apiUrl,
				queryStrings: queryStrings,
				parameters: parameters,
				logPrefix: nameof(SignOutUserAsync),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!IsSuccessStatusCode)
		{
			throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})");
		}
	}

	public async ValueTask<bool> VerifyTransferCreditAsync(Uri apiUrl, string appId, string secretKey, string requestId, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "TCH");
				writer.WriteString("RequestID", requestId);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, _) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(VerifyTransferCreditAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return IsSuccessStatusCode && HttpStatusCode != HttpStatusCode.NotFound;
	}

	public async ValueTask<TransferCreditResult> WithdrawAsync(Uri apiUrl, string appId, string secretKey, string username, string requestId, decimal amount, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "TC");
				writer.WriteString("Username", username);
				writer.WriteString("RequestID", requestId);
				writer.WriteNumber("Amount", -amount);
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(WithdrawAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !IsSuccessStatusCode
			? throw new Exception($"{JsonNode?["Message"]}( {(int)HttpStatusCode})")
			: new TransferCreditResult(
			JsonNode!["RequestID"]!.GetValue<string>(),
			JsonNode.ToJsonString());
	}

	public async ValueTask<JsonNode> DomainGetWhitelistAsync(Uri apiUrl, string appId, string secretKey, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "GDS");
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(DomainGetWhitelistAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return JsonNode!;
	}

	public async ValueTask<JsonNode> DomainRegisterWhitelistAsync(Uri apiUrl, string appId, string secretKey, IEnumerable<string> domains, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "RDS");
				writer.WriteString("Domain", string.Join(",", domains));
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(DomainRegisterWhitelistAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return JsonNode!;
	}

	public async ValueTask<JsonNode> DomainUnRegisterWhitelistAsync(Uri apiUrl, string appId, string secretKey, IEnumerable<string> domains, CancellationToken cancellationToken = default)
	{
		var parameters = default(JsonNode);

		using (var stream = new MemoryStream())
		{
			using (var writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartObject();
				writer.WriteString("Method", "UDS");
				writer.WriteString("Domain", string.Join(",", domains));
				writer.WriteNumber("Timestamp", timeProvider.GetUtcNow().ToUnixTimeSeconds());
				writer.WriteEndObject();
			}

			parameters = System.Text.Json.Nodes.JsonNode.Parse(Encoding.UTF8.GetString(stream.ToArray()))!;
		}

		var signature = signatureUtensil.GenerateSignature(source: parameters, secretKey: secretKey);

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);
		queryStrings.Add("appid", appId);
		queryStrings.Add("signature", signature);

		var (IsSuccessStatusCode, HttpStatusCode, JsonNode) = await PostAsync(
			apiUrl: apiUrl,
			queryStrings: queryStrings,
			parameters: parameters,
			logPrefix: nameof(DomainUnRegisterWhitelistAsync),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return JsonNode!;
	}

	private async ValueTask<(bool IsSuccessStatusCode, HttpStatusCode HttpStatusCode, JsonNode? JsonNode)> PostAsync(Uri apiUrl, NameValueCollection queryStrings, JsonNode parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		var sw = Stopwatch.StartNew();

		var url = $"{apiUrl.OriginalString}?{queryStrings}";
		var content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
		var httpResponseMessage = await client
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

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestParameters = parameters.ToJsonString(),
			RequestContent = (await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		return new(httpResponseMessage.IsSuccessStatusCode, httpResponseMessage.StatusCode, (!string.IsNullOrEmpty(responseMessage)) ? System.Text.Json.Nodes.JsonNode.Parse(responseMessage) : null);
	}
}
