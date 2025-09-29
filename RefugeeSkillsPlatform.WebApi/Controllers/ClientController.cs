using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.WebApi.Common;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }
        [HttpGet]
        public IActionResult GetAllAvailableServiceSlots(ServiceSlotRequest request)
        {
            var result =  _clientService.GetServiceSlots(request);
            if(result == null || !result.Any())
            {
                return Ok( new StandardRequestResponse<List<ServiceSlotResponse>>()
                {
                    Data = null,
                    Success = false,
                    Status = 401,
                    Message = "No Record found"

                });
            }
            return Ok(new StandardRequestResponse<List<ServiceSlotResponse>>()
            {
                Data = result,
                Success = true,
                Status = 200,
                Message = "Successfully fetch data"

            });
        }
        [HttpPost]
        public IActionResult Booking([FromBody] BookingDTO request)
        {
            if(!_clientService.CreateBooking(request))
            {
                return Ok(new StandardRequestResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Status = 401,
                    Message = "Booking Failed"

                });

            }
            return Ok(new StandardRequestResponse<bool>()
            {
                Data = true,
                Success = true,
                Status = 200,
                Message = "Service Booked Successfully"

            });
        }
    }
}
