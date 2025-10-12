using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.WebApi.Common;
using System.Security.Claims;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;
        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration; 
        }
        [HttpPost]
        public IActionResult Register([FromBody] UserRequest request)
        {
            var result = _userService.RegisterUser(request);
            if (result == 0)
            {
                var response = new StandardRequestResponse<int>()
                {
                    Data = 0,
                    Success = false,
                    Message = "User Name or Email already exist.",
                    Status = 400
                };
                return Ok(response);
            }
            else
            {
                var response = new StandardRequestResponse<int>()
                {
                    Data = 1,
                    Success = true,
                    Message = "Register successfully.",
                    Status = 200
                };
                return Ok(response);
            }
        }

        [HttpPost]
        public IActionResult Login([FromBody] Credentials credential)
        {
            var userProfile = _userService.VerifyUser(credential);
           // if (credential.UserName == "Ishtiaq" && credential.Password == "password")
           if(userProfile != null)
            {
                var claims = new List<Claim>() {
                new Claim (ClaimTypes.Name, userProfile.UserName),
                new Claim( ClaimTypes.Email, userProfile.Email),
                new Claim("userName", userProfile.UserName),
                new Claim("email", userProfile.Email),
                new Claim( "role", userProfile.RoleName)
                };


                var expiresAt = DateTime.UtcNow.AddMinutes(30);
                return Ok(new
                {
                    user_id = userProfile.UserId,
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt,
                });
            }
            ModelState.AddModelError("Unauthorized", "You are not Authorized to access the end point.");
            var problemDetails = new ProblemDetails()
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
            };
            return Unauthorized(problemDetails);
        }

        private string CreateToken(List<Claim> claims, DateTime expiresAt)
        {

            var claimDic = new Dictionary<string, object>();
            if (claims is not null && claims.Count > 0)
            {
                foreach (var claim in claims)
                {
                    claimDic.Add(claim.Type, claim.Value);
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Claims = claimDic,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["SecretKey"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256Signature),
                NotBefore = DateTime.UtcNow
            };
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

    }
}
