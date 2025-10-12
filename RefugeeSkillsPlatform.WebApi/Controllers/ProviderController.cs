using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.WebApi.Common;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        public ProviderController(IProviderService providerService)
        {
            _providerService = providerService; 
        }
        [HttpPost]
        public IActionResult CreateService(ServiceRequest request)
        {
            if (_providerService.CreateSerice(request) == 0)
            {
                return Ok(new StandardRequestResponse<ServiceRequest>() {
                    Data= null,
                    Message= "Failed to Create Service",
                    Status= 404,
                    Success = false
                }
                
                );
            }
            else
            {
                return Ok(new StandardRequestResponse<ServiceRequest>()
                {
                    Data = request,
                    Message = "Successfully Create Service",
                    Status = 200,
                    Success = true
                });
            }
        }
        [HttpPost]
        public IActionResult CreateSlots(AvailabilitySlotsDTO request)
        {

            if (_providerService.CreateSlots(request) == 0)
            {
                return Ok(new StandardRequestResponse<AvailabilitySlotsDTO>()
                {
                    Data = null,
                    Message = "Failed to Create slots",
                    Status = 404,
                    Success = false
                }

                );
            }
            else
            {
                return Ok(new StandardRequestResponse<AvailabilitySlotsDTO>()
                {
                    Data = request,
                    Message = "Successfully Create slots",
                    Status = 200,
                    Success = true
                });
            }
        }

        //sp for getting all bookings =  Sp_GetAllBookings

        // Check Account Approved or Not
        [HttpGet]
        public IActionResult IsAccountApproved(int providerId)
        {
            var isApproved = _providerService.IsAccountApproved(providerId);
            return Ok(new StandardRequestResponse<bool>()
            {
                Data = isApproved,
                Message = isApproved ? "Account is approved" : "Account is not approved",
                Status = 200,
                Success = true
            });
        }
    }
}
