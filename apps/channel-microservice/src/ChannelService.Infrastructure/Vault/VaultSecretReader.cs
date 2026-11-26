using System.Net;
using System.Text.Json;

namespace ChannelService.Infrastructure.Vault;

public static class VaultSecretReader
{
    public static async Task<string> GetDatabasePasswordAsync()
    {
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

        using var response = await client.GetAsync("v1/secret/data/channel-database");

        if (response.StatusCode == HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Vault denied access: check token, TTL and policy.");

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var password = document.RootElement
            .GetProperty("data")
            .GetProperty("data")
            .GetProperty("Password")
            .GetString();

        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Vault secret Password is missing or empty.");

        return password;
    }
}