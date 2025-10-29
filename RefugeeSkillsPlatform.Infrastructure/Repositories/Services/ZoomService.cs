using Microsoft.Extensions.Configuration;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories.Services
{
    public class ZoomService : IZoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public ZoomService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _httpClient = new HttpClient();

            _clientId = _config["ZoomSettings:ClientId"] ?? string.Empty;
            _clientSecret = _config["ZoomSettings:ClientSecret"] ?? string.Empty;
            _redirectUri = _config["ZoomSettings:RedirectUri"] ?? string.Empty;
        }

        public string GenerateZoomAuthorizationUrl(long userId)
        {
            var baseUrl = "https://zoom.us/oauth/authorize";
            var state = Convert.ToString(userId); // Use userId as state, or generate a random string
            return $"{baseUrl}?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}&state={state}";
        }

        public async Task<ZoomTokenResponse?> ExchangeCodeForTokensAsync(string code, string state)
        {
            var tokenUrl = "https://zoom.us/oauth/token";
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, $"{tokenUrl}?grant_type=authorization_code&code={code}&redirect_uri={_redirectUri}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                var response = await _httpClient.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();




                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve access token from Zoom. " +
                                        $"Status: {response.StatusCode}, Body: {json}");
                }

            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<ZoomTokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenData == null)
                throw new Exception("Invalid token response.");

                // Fetch user info from Zoom
                var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/v2/users/me");
                userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
                var userInfoResponse = await _httpClient.SendAsync(userInfoRequest);
                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();

                var userInfo = JsonSerializer.Deserialize<ZoomUserInfo>(userInfoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


                // Parse userId from state
                if (!long.TryParse(state, out var userId))
                    throw new Exception($"Invalid state value '{state}' — expected userId.");

                // Save or update Zoom account
                var zoomRepo = _unitOfWork.GetRepository<UserZoomAccount>();
                var existing = zoomRepo.FirstOrDefult(x => x.UserId == userId);

                if (existing == null)
                {
                    zoomRepo.Add(new UserZoomAccount
                    {
                        UserId = userId,
                        AccessToken = tokenData.AccessToken,
                        RefreshToken = tokenData.RefreshToken,
                        TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn),
                        ZoomEmail = userInfo?.Email,
                        //ZoomAccountId = userInfo?.Id
                    });
                }
                else
                {
                    existing.AccessToken = tokenData.AccessToken;
                    existing.RefreshToken = tokenData.RefreshToken;
                    existing.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn);
                    existing.ZoomEmail = userInfo?.Email;
                    //existing.ZoomAccountId = userInfo?.Id;
                    zoomRepo.Update(existing);
                }

            _unitOfWork.Commit();
            return tokenData;
        }

        public async Task<ZoomAccountResponse?> GetUserZoomAccountAsync(long userId)
        {
            var repo = _unitOfWork.GetRepository<UserZoomAccount>();
            var account = repo.FirstOrDefult(x => x.UserId == userId);

            if (account == null)
                return null;

            return new ZoomAccountResponse
            {
                UserId = account.UserId,
                ZoomEmail = account.ZoomEmail,
                TokenExpiresAt = account.TokenExpiresAt
            };
        }

        public async Task<ZoomTokenResponse?> RefreshAccessTokenAsync(long userId)
        {
            var repo = _unitOfWork.GetRepository<UserZoomAccount>();
            var account = repo.FirstOrDefult(x => x.UserId == userId);

            if (account == null)
                throw new Exception("Zoom account not found.");

            var tokenUrl = "https://zoom.us/oauth/token";
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, $"{tokenUrl}?grant_type=refresh_token&refresh_token={account.RefreshToken}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to refresh access token.");

            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<ZoomTokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenData == null)
                throw new Exception("Invalid refresh token response.");

            account.AccessToken = tokenData.AccessToken;
            account.RefreshToken = tokenData.RefreshToken;
            account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn);

            repo.Update(account);
            _unitOfWork.Commit();

            return tokenData;
        }

        public async Task<bool> DisconnectZoomAccountAsync(long userId)
        {
            var repo = _unitOfWork.GetRepository<UserZoomAccount>();
            var account = repo.FirstOrDefult(x => x.UserId == userId);

            if (account == null)
                return false;

            //repo.Remove(account);
            _unitOfWork.Commit();
            return true;
        }
    }
}
