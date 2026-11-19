using System.Net;
using System.Text.Json;

var address = Environment.GetEnvironmentVariable("VAULT_ADDR")
    ?? "http://localhost:8200";
var token = Environment.GetEnvironmentVariable("VAULT_TOKEN")
    ?? throw new InvalidOperationException("Set VAULT_TOKEN to the scoped application token.");
using var client = new HttpClient
{
    BaseAddress = new Uri(address.TrimEnd('/') + "/"),
    Timeout = TimeSpan.FromSeconds(5)
};
client.DefaultRequestHeaders.Add("X-Vault-Token", token);
using var response = await client.GetAsync("v1/secret/data/pingpong");
if (response.StatusCode == HttpStatusCode.Forbidden)
    throw new InvalidOperationException("Vault denied access: check token, TTL and policy.");
response.EnsureSuccessStatusCode();
using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
var apiKey = document.RootElement.GetProperty("data").GetProperty("data")
    .GetProperty("ApiKey").GetString();
if (string.IsNullOrWhiteSpace(apiKey))
    throw new InvalidOperationException("Vault secret ApiKey is missing or empty.");
// Use apiKey to configure the downstream client; never log or return its value.
Console.WriteLine("Vault secret loaded successfully (value not displayed).");
