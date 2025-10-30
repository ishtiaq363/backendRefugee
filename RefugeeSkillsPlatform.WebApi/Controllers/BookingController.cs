using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IClientService clientService, IUnitOfWork unitOfWork)
        {
            _clientService = clientService;
            _unitOfWork = unitOfWork;
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

        [HttpPost]
        public IActionResult UpdatePaymentStatus([FromBody] PaymentStatusUpdateRequest request)
        {
            //request.Status = "Pending";
            var result = _clientService.UpdatePaymentStatus(request.BookingId, request.Status);

            if (result == 0)
                return NotFound("Booking not found");

            return Ok(new { message = "Payment status updated successfully" });
        }

        [HttpPost]
        public IActionResult RefundPayment([FromBody] RefundBookingRequest request)
        {
            try
            {
                // Step 1: Find the payment record by BookingId
                var payment = _unitOfWork.GetRepository<Payments>().FirstOrDefult(p => p.BookingId == request.BookingId);
                var booking = _unitOfWork.GetRepository<Bookings>().FirstOrDefult(b => b.BookingId == request.BookingId);
                if (payment == null)
                    return NotFound("Payment not found for this booking.");

                if (payment.PaymentStatus != "Paid")
                    return BadRequest("This payment is not eligible for refund.");

                // Step 2: Get PaymentIntent (using Stripe)
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Get(payment.TransactionReference);

                // Step 3: Get the latest charge ID
                var chargeId = paymentIntent.LatestChargeId;
                if (string.IsNullOrEmpty(chargeId))
                    return BadRequest("No charge found for this payment intent.");

                // Step 4: Create refund
                var refundService = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    Charge = chargeId,
                    Amount = (long)(payment.Amount * 100), // refund full amount
                    Reason =  "requested_by_customer"
                };

                var refund = refundService.Create(refundOptions);

                // Step 5: Update DB
                payment.IsRefunded = true;
                payment.RefundAmount = payment.Amount;
                payment.RefundDate = DateTime.UtcNow;
                payment.RefundReference = refund.Id;
                payment.RefundReason = request.RefundReason ?? "requested_by_customer";
                payment.PaymentStatus = "Refunded";

                booking.Status = "Refunded";
                _unitOfWork.Commit();

                return Ok(new
                {
                    Message = "Refund processed successfully.",
                    RefundId = refund.Id,
                    RefundStatus = refund.Status
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
