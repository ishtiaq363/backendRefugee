using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces.Services
{
    public interface IZoomService
    {
        string GenerateZoomAuthorizationUrl(long userId);

        Task<ZoomTokenResponse?> ExchangeCodeForTokensAsync(string code, string state);

        Task<ZoomAccountResponse?> GetUserZoomAccountAsync(long userId);

        Task<ZoomTokenResponse?> RefreshAccessTokenAsync(long userId);

        Task<bool> DisconnectZoomAccountAsync(long userId);
    }
}
