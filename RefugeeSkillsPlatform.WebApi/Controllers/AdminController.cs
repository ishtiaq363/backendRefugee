using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.WebApi.Common;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProviderService _providerService;
        public AdminController(IUserService userService, IProviderService providerService)
        {
               _userService = userService;
            _providerService = providerService;
        }
        [HttpPost]
        public IActionResult GetAllUsers([FromBody] UserProfileRequest userProfileRequest)
        {
            var userList = _userService.GetUserProfiles(userProfileRequest);
            if (userList == null || !userList.Any())
            {
                return Ok(new StandardRequestResponse<List<UserProfileResponse>>()
                {
                    Data = null,
                    Status = 404,
                    Success = false,
                    Message = "No record to show"
                });
            }
            return Ok(new StandardRequestResponse<List<UserProfileResponse>>() {            
            Data = userList,
            Status = 200,
            Success = true,
            Message ="Successfully retrieve the Users"
            });

        }
        [HttpPost]
        public IActionResult ProviderApproval([FromBody] ApprovalRequest request)
        {
            var result = _userService.ProviderApproval(request);
            if(!result)
            {
                return Ok(new StandardRequestResponse<bool>()
                {
                    Success =false,
                    Message ="Failed to Approve",
                    Data = false,
                    Status = 404
                });
            }
            return Ok(new StandardRequestResponse<bool>()
            {
                Success = true,
                Message = "Approved sucessfully",
                Data = true,
                Status = 200
            });
        }

        [HttpPost]
        public IActionResult GetAllServices([FromBody] AllServicesRequest request)
        {
            var servicesList = _providerService.GetAllServices(request);
            if (servicesList == null || !servicesList.Any())
            {
                return NotFound(new StandardRequestResponse<List<ServiceResponse>>()
                {
                    Data = null,
                    Status = 404,
                    Success = false,
                    Message = "No record to show"
                });
            }
            return Ok(new StandardRequestResponse<List<ServiceResponse>>()
            {
                Data = servicesList,
                Status = 200,
                Success = true,
                Message = "Successfully retrieve the Users"
            });

        }

        [HttpPost]
        public IActionResult GetProviderDashboardStats(ProviderStatsRequest request)
        {
            var providerStats = _providerService.GetProviderStats(request);
            if (providerStats == null)
            {
                return NotFound(new StandardRequestResponse<ProviderStatResponse>()
                {
                    Data = null,
                    Status = 404,
                    Success = false,
                    Message = "No record to show"
                });
            }
            return Ok(new StandardRequestResponse<ProviderStatResponse>()
            {
                Data = providerStats,
                Status = 200,
                Success = true,
                Message = "Successfully retrieve the Users"
            });

        }


        [HttpGet]
        public IActionResult GetAdminDashboardStats()
        {
            var adminStats = _providerService.GetAdminStats();
            if (adminStats == null)
            {
                return NotFound(new StandardRequestResponse<AdminStatResponse>()
                {
                    Data = null,
                    Status = 404,
                    Success = false,
                    Message = "No record to show"
                });
            }
            return Ok(new StandardRequestResponse<AdminStatResponse>()
            {
                Data = adminStats,
                Status = 200,
                Success = true,
                Message = "Successfully retrieve the Users"
            });

        }

        [HttpPost]
        public IActionResult ServiceApproval([FromBody] ServiceApprovalRequest request)
        {
            var result = _userService.ServiceApproval(request);
            if (!result)
            {
                return NotFound(new StandardRequestResponse<bool>()
                {
                    Success = false,
                    Message = "Failed to Approve Service, Service Not fount with this Id",
                    Data = false,
                    Status = 404
                });
            }
            return Ok(new StandardRequestResponse<bool>()
            {
                Success = true,
                Message = "Service Approved sucessfully",
                Data = true,
                Status = 200
            });
        }

        [HttpPost]
        public IActionResult GetAllBookings([FromBody] AllBookingsRequest request)
        {
            var bookingList = _providerService.GetBookingList(request);
            if (bookingList == null || bookingList.Count == 0)
            {
                return NotFound(new StandardRequestResponse<List<BookingListDto>>()
                {
                    Data = null,
                    Status = 404,
                    Success = false,
                    Message = "No record to show"
                });
            }
            return Ok(new StandardRequestResponse<List<BookingListDto>>()
            {
                Data = bookingList,
                Status = 200,
                Success = true,
                Message = "Successfully retrieve the Users"
            });

        }
    }
}
