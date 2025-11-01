using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.Infrastructure.Repositories.Services;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoomController : ControllerBase
    {
        private readonly IZoomService _zoomService;
        private readonly IUserService _userService;

        public ZoomController(IZoomService zoomService, IUserService userService)
        {
            _zoomService = zoomService;
            _userService = userService;
        }

        /// <summary>
        /// Step 1: Redirects user to Zoom OAuth authorization page.
        /// </summary>
        [HttpGet("authorize/{userId}")]
        public IActionResult AuthorizeZoom(long userId)
        {
            try
            {
                var redirectUrl = _zoomService.GenerateZoomAuthorizationUrl(userId);
                return Ok(new
                {
                    success = true,
                    data = new { redirectUrl }
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Step 2: Handles Zoom OAuth callback (redirect URL).
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> ZoomCallback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest(new { success = false, message = "Authorization code is missing." });

                var result = await _zoomService.ExchangeCodeForTokensAsync(code, state);
                return Ok(new
                {
                    success = true,
                    message = "Zoom account connected successfully.",
                    data = "Success"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Step 3: Get Zoom account info of a specific user
        /// </summary>

        [HttpGet("account/{userId}")]
        public async Task<IActionResult> GetZoomAccount(long userId)
        {
            try
            {
                var account = await _zoomService.GetUserZoomAccountAsync(userId);
                if (account == null)
                    return NotFound(new { success = false, message = "No Zoom account linked." });

                return Ok(new { success = true, data = account });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Step 4: Refresh the access token (optional)
        /// </summary>

        [HttpPost("refresh/{userId}")]
        public async Task<IActionResult> RefreshAccessToken(long userId)
        {
            try
            {
                var result = await _zoomService.RefreshAccessTokenAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Step 5: Disconnect Zoom account
        /// </summary>

        [HttpDelete("disconnect/{userId}")]
        public async Task<IActionResult> DisconnectZoom(long userId)
        {
            try
            {
                var result = await _zoomService.DisconnectZoomAccountAsync(userId);
                return Ok(new { success = result, message = result ? "Zoom disconnected successfully." : "Failed to disconnect Zoom." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("create-meeting/{userId}")]
        public async Task<IActionResult> CreateMeeting(long userId, [FromBody] ZoomMeetingRequest request)
        {
            try
            {
                var profile = _userService.GetUserProfileById(userId);
                if (profile == null)
                    return BadRequest(new { success = false, message = "User not found." });
                else if (profile.RoleName != "Provider")
                    return BadRequest(new { success = false, message = "Only providers can create Zoom meetings." });
                else if (!profile.IsApproved)
                    return BadRequest(new { success = false, message = "Provider account not approved." });

                var exist = await _zoomService.GetUserZoomAccountAsync(userId);
                if (exist == null)
                    return BadRequest(new { success = false, message = "Zoom account not connected." });
                else if (exist.TokenExpiresAt <= DateTime.UtcNow){
                    await _zoomService.RefreshAccessTokenAsync(userId);
                }   

                var meeting = await _zoomService.CreateMeetingAsync(userId, request);
                return Ok(new { success = true, data = meeting });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}

