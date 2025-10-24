using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.Infrastructure.Repositories;
using RefugeeSkillsPlatform.Infrastructure.Repositories.Services;
using RefugeeSkillsPlatform.WebApi.Common;
using Stripe;

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


        [HttpPost]
        public IActionResult CreatePaymentIntent([FromBody] PaymentRequest request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = "usd",
                Metadata = new Dictionary<string, string>
        {
            { "BookingId", request.BookingId.ToString() }
        }
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }
        //[HttpPost]
        //public IActionResult CreatePaymentIntent([FromBody] PaymentRequest request)
        //{
        //    var options = new PaymentIntentCreateOptions
        //    {
        //        Amount = (long)(request.Amount * 100),
        //        Currency = "usd",
        //        Metadata = new Dictionary<string, string>
        //        {
        //            { "BookingId", request.BookingId.ToString() }
        //        }
        //    };

        //    var service = new PaymentIntentService();
        //    var paymentIntent = service.Create(options);

        //    return Ok(new { clientSecret = paymentIntent.ClientSecret });
        //}

        [HttpPost]
        public IActionResult UpdatePaymentStatus([FromBody] PaymentStatusUpdateRequest request)
        {
            var result = _clientService.UpdatePaymentStatus(request.BookingId, request.Status);

            if (result == 0)
                return NotFound("Booking not found");

            return Ok(new { message = "Payment status updated successfully" });
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
