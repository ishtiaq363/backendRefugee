using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.Infrastructure.Repositories.Services;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoomController : ControllerBase
    {
        private readonly IZoomService _zoomService;

        public ZoomController(IZoomService zoomService)
        {
            _zoomService = zoomService;
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
                    redirectUrl
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
                    data = result
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
    }
}

