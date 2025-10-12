using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.Infrastructure.Repositories;
using RefugeeSkillsPlatform.Infrastructure.Repositories.Services;
using RefugeeSkillsPlatform.WebApi.Common;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IClientService _clientService;
        public BookingController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("{availabilitySlotId}")]
        public IActionResult GetBookedSlots(int availabilitySlotId, [FromQuery] DateTime date)
        {
            var bookings = _clientService.GetBookedSlots(availabilitySlotId, date);

            if (bookings == null || !bookings.Any())
            {
                return Ok(new StandardRequestResponse<List<BookedSlotResponse>>()
                {
                    Data = null,
                    Message = "No booked slots found for the selected date.",
                    Status = 404,
                    Success = false
                });
            }

            return Ok(new StandardRequestResponse<List<BookedSlotResponse>>()
            {
                Data = bookings,
                Message = "Booked slots retrieved successfully.",
                Status = 200,
                Success = true
            });
        }


        //[HttpGet("booked-slots/{availabilitySlotId}")]
        //public IActionResult GetBookedSlots(int availabilitySlotId, DateTime date)
        //{
        //    var bookings = _clientService.GetBookedSlots(availabilitySlotId, date);

        //    if (bookings == null || !bookings.Any())
        //    {
        //        return Ok(new StandardRequestResponse<List<BookedSlotResponse>>()
        //        {
        //            Data = null,
        //            Message = "No booked slots found for the selected date.",
        //            Status = 404,
        //            Success = false
        //        });
        //    }

        //    return Ok(new StandardRequestResponse<List<BookedSlotResponse>>()
        //    {
        //        Data = bookings,
        //        Message = "Booked slots retrieved successfully.",
        //        Status = 200,
        //        Success = true
        //    });
        //}

    }
}
