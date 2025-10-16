using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.Infrastructure.Repositories.Services;
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
        [HttpPost]
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
                    Message = "No Slots Available"

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
        public IActionResult GetAllBookingsForClient([FromBody] BookingRequestForClient request)
        {
            var bookingList = _clientService.GetBookingListForClientId(request);
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
                    Message = "Booking Failed, already booked"

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

        [HttpPost]
        public IActionResult CreatePayment(PaymentDto request)
        {
            if (_clientService.CreatePayment(request) == 0)
            {
                return Ok(new StandardRequestResponse<int>()
                {
                    Message = "Failed to make a Payment",
                    Status = 401,
                    Data = 0,
                    Success = false,
                });
            }
            return Ok(new StandardRequestResponse<int>()
            {
                Message = "Payment done successfully",
                Status = 200,
                Data = 1,
                Success= true
            });
        }

        [HttpPost]
        public IActionResult GetClientDashboardStats(ClientStatsRequest request)
        {
            var clientStats = _clientService.GetClientStats(request);
            if (clientStats == null)
            {
                return NotFound(new StandardRequestResponse<ClientStatResponse>()
                {
                    Data = null,
                    Status = 404,
                    Success = false,
                    Message = "No record to show"
                });
            }
            return Ok(new StandardRequestResponse<ClientStatResponse>()
            {
                Data = clientStats,
                Status = 200,
                Success = true,
                Message = "Successfully retrieve the Users"
            });

        }


    }
}
