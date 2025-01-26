namespace RoomService;

using System.Net.Http;
using System.Text.Json;

public class AuthServiceClient
{
    private readonly HttpClient _httpClient;

    public AuthServiceClient(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("AuthService");
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/validate");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }
    
  
}
