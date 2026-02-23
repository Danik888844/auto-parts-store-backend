using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AutoParts.Business.Services.Identity;

public class IdentityUserApiClient : IIdentityUserApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityUserApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Dictionary<string, string>> GetDisplayNamesAsync(IReadOnlyList<string> userIds, CancellationToken cancellationToken = default)
    {
        if (userIds == null || userIds.Count == 0)
            return new Dictionary<string, string>();

        var request = new HttpRequestMessage(HttpMethod.Post, "api/user/display-names");
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

        request.Content = new StringContent(
            JsonConvert.SerializeObject(new { UserIds = userIds }),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var wrapper = JsonConvert.DeserializeObject<IdentityDisplayNamesResponse>(json);
        return wrapper?.Data ?? new Dictionary<string, string>();
    }

    private class IdentityDisplayNamesResponse
    {
        [JsonProperty("data")]
        public Dictionary<string, string> Data { get; set; } = new();
    }
}
