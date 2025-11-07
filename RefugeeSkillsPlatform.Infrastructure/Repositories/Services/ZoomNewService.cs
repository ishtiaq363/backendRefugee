using Microsoft.Extensions.Configuration;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories.Services
{
    public class ZoomNewService : IZoomNewService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ZoomNewService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var accountId = _config["Zoom:AccountId"];
            var clientId = _config["Zoom:ClientId"];
            var clientSecret = _config["Zoom:ClientSecret"];

            using var client = new HttpClient();
            var tokenUrl = $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}";

            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await client.PostAsync(tokenUrl, null);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            return content.GetProperty("access_token").GetString();
        }

        public async Task<string> CreateMeetingAsync(string topic, DateTime startTime)
        {
            var accessToken = await GetAccessTokenAsync();

            var meetingData = new
            {
                topic = topic,
                type = 2, // scheduled meeting
                start_time = startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = 60,
                timezone = "Asia/Karachi",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = false
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.zoom.us/v2/users/me/meetings");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(meetingData);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("join_url").GetString();
        }
    }
}
