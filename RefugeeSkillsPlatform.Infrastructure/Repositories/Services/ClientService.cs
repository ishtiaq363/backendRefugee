using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories.Services
{
    public class MeetingRequest
    {
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
    }
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IZoomService _zoomService;
        private readonly IZoomNewService _zoomNewService;

        public ClientService(IUnitOfWork unitOfWork, IZoomService zoomService , IZoomNewService zoomNewService )
        {
            _unitOfWork = unitOfWork;
            _zoomService = zoomService;
            _zoomNewService = zoomNewService;
        }

        public long CreateBooking(BookingDTO request)
        {
            
                var isAlreadyBooked = _unitOfWork.GetRepository<Bookings>().GetAll().Any(x =>
                x.AvailabilitySlotId == request.AvailabilitySlotId &&
                x.SlotStartDate.Date == request.SlotStartDate.Date &&
                x.SlotEndDate.Date == request.SlotEndDate.Date &&
                (
                    (request.BookingStart >= x.BookingStart && request.BookingStart < x.BookingEnd) ||
                    (request.BookingEnd > x.BookingStart && request.BookingEnd <= x.BookingEnd) ||
                    (request.BookingStart <= x.BookingStart && request.BookingEnd >= x.BookingEnd) ||
                    (x.BookingStart == x.BookingEnd && request.BookingStart == x.BookingStart) 
                )
            );



            if (isAlreadyBooked)
                return 0; // booking conflict

            var client = _unitOfWork.GetRepository<Users>().FirstOrDefult(u => u.Email == request.Email);
            if (client is null)
            {
                throw new InvalidOperationException("Client not found for the provided email.");
            }
            // If not already booked, save the booking
            var newBooking = new Bookings
            {
                AvailabilitySlotId = request.AvailabilitySlotId,
                ClientId = client.UserId,
                BookingStart = request.BookingStart,
                BookingEnd = request.BookingEnd,
                SlotStartDate = request.SlotStartDate,
                SlotEndDate = request.SlotEndDate,
                Status = request.Status,
                BookingDate = request.BookingDate,
                ZoomLink = request.ZoomLink
            };

            _unitOfWork.GetRepository<Bookings>().Add(newBooking);
            _unitOfWork.Commit();

            return newBooking.BookingId;
        }


        public List<ServiceSlotResponse> GetServiceSlots(ServiceSlotRequest request)
        {
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var serviceIdParam = new SqlParameter("@ServiceId", SqlDbType.Int)
            {
                Value = request.ServiceId.HasValue ? (object)request.ServiceId.Value : DBNull.Value
            };

            var services = _unitOfWork.SpListRepository<ServiceSlotResponse>(
                "sp_GetAllServiceSlots @PageNumber, @PageSize, @ServiceId",
                 pageNumParam, pageSizeParam, serviceIdParam);

            return services.Any() ? services : new List<ServiceSlotResponse>();
        }


        public async Task<int> CreatePaymentAsync(PaymentDto request)
        {
            var result = _unitOfWork.GetRepository<Payments>().FirstOrDefult(p => p.BookingId == request.BookingId);
            if (result != null)
            {
                return 0;
            }
            var payment = new Payments()
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                PaymentStatus = "Paid",
                TransactionReference = request.TransactionReference,
                PaymentMethod = request.PaymentMethod
            };
            _unitOfWork.GetRepository<Payments>().Add(payment);
            // Create & Add Zoom Link
            string status = "";
            var booking = _unitOfWork.GetRepository<Bookings>()
                .FirstOrDefult(b => b.BookingId == request.BookingId);

            if (booking == null)
                throw new Exception("Booking not found.");

            // 2️⃣ Get the service ID for that slot
            var slot = _unitOfWork.GetRepository<AvailabilitySlots>()
                .FirstOrDefult(a => a.AvailabilitySlotId == booking.AvailabilitySlotId);

            if (slot == null)
                throw new Exception("Availability slot not found.");

            // 3️⃣ Get the CreatedByUserId for that service
            var service = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>()
                .FirstOrDefult(s => s.ServiceId == slot.ServiceId);

            if (service == null)
                throw new Exception("Service not found.");

            var createdByUserId = service.CreatedByUserId;



            if (createdByUserId == 0)
            {
                status = "Provider not found.";
            }
            else
            {
                var meetingUrl = await _zoomNewService.CreateMeetingAsync(service?.ServiceName?.ToString(), booking.SlotStartDate);

                
                if (meetingUrl != null)
                {
                    booking.ZoomLink = meetingUrl;
                    booking.ZoomStatus = "Zoom Link Generated";
                    
                }
                else
                {
                    booking.ZoomStatus = status;
                }


            }
            _unitOfWork.GetRepository<Bookings>().Update(booking);






            return _unitOfWork.Commit();
        }

        //public async Task<int> CreatePaymentAsync(PaymentDto request)
        //{
        //    var result = _unitOfWork.GetRepository<Payments>().FirstOrDefult(p => p.BookingId == request.BookingId);
        //    if (result != null)
        //    {
        //        return 0;
        //    }
        //    var payment = new Payments()
        //    {
        //        BookingId = request.BookingId,
        //        Amount = request.Amount,
        //        PaymentDate = request.PaymentDate,
        //        PaymentStatus = "Paid",
        //        TransactionReference = request.TransactionReference,
        //        PaymentMethod = request.PaymentMethod
        //    };
        //    _unitOfWork.GetRepository<Payments>().Add(payment);
        //    // Create & Add Zoom Link
        //    string status = "";
        //    var booking = _unitOfWork.GetRepository<Bookings>()
        //        .FirstOrDefult(b => b.BookingId == request.BookingId);

        //    if (booking == null)
        //        throw new Exception("Booking not found.");

        //    // 2️⃣ Get the service ID for that slot
        //    var slot = _unitOfWork.GetRepository<AvailabilitySlots>()
        //        .FirstOrDefult(a => a.AvailabilitySlotId == booking.AvailabilitySlotId);

        //    if (slot == null)
        //        throw new Exception("Availability slot not found.");

        //    // 3️⃣ Get the CreatedByUserId for that service
        //    var service = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>()
        //        .FirstOrDefult(s => s.ServiceId == slot.ServiceId);

        //    if (service == null)
        //        throw new Exception("Service not found.");

        //    var createdByUserId = service.CreatedByUserId;



        //    if (createdByUserId == 0)
        //    {
        //        status = "Provider not found.";
        //    }
        //    else
        //    {
        //        var profile = _unitOfWork.GetRepository<Users>().FirstOrDefult(u => u.UserId == createdByUserId);
        //        if (profile == null)
        //            status = "Provider not found.";
        //        else if (profile.RoleId != 4002)
        //            status = "Only providers can host Zoom meetings.";
        //        else if (!profile.IsApproved)
        //            status = "Provider account is not approved to create Zoom meetings.";
        //        var exist = await _zoomService.GetUserZoomAccountAsync(createdByUserId);
        //        if (exist == null)
        //            status = "No Zoom account linked for the provider.";
        //        else if (exist.TokenExpiresAt <= DateTime.UtcNow)
        //        {
        //            await _zoomService.RefreshAccessTokenAsync(createdByUserId);
        //        }
        //        ZoomMeetingRequest zoomMeetingRequest = new ZoomMeetingRequest
        //        {
        //            Topic = "Meeting for Booking ID: " + request.BookingId,
        //            StartTime = DateTime.Today.Add(booking.BookingStart),
        //            Duration = 30,
        //            ZoomPassword = false
        //        };
        //        var meeting = await _zoomService.CreateMeetingAsync(createdByUserId, zoomMeetingRequest);
        //        status = meeting != null ? "Zoom meeting created successfully." : "Failed to create Zoom meeting.";
        //        if (meeting != null)
        //        {
        //            booking.ZoomLink = meeting.JoinUrl;
        //            booking.ZoomStatus = "Zoom Link Generated";

        //        }
        //        else
        //        {
        //            booking.ZoomStatus = status;
        //        }


        //    }
        //    _unitOfWork.GetRepository<Bookings>().Update(booking);






        //    return _unitOfWork.Commit();
        //}

        public List<BookedSlotResponse> GetBookedSlots(int availabilitySlotId, DateTime date)
        {
            var bookings = _unitOfWork.GetRepository<Bookings>()
                .GetAll()
                .Where(b => b.AvailabilitySlotId == availabilitySlotId
                            && b.SlotStartDate.Date == date.Date)
                .Select(b => new BookedSlotResponse
                {
                    BookingStart = b.BookingStart,
                    BookingEnd = b.BookingEnd
                })
                .ToList();

            return bookings.Any() ? bookings : new List<BookedSlotResponse>();
        }

       
        public List<BookingListDto> GetBookingListForClientId(BookingRequestForClient request)
        {
            var currentClient = _unitOfWork.GetRepository<Users>().FirstOrDefult(u => u.Email == request.Email);
            if(currentClient is null)
            {
                throw new InvalidOperationException("Invalid email of the Client email.");
            }
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var clientId = new SqlParameter("@ClientId", SqlDbType.BigInt) { Value = (object?)currentClient.UserId ?? DBNull.Value };
            var services = _unitOfWork.SpListRepository<BookingListDto>(
           "Sp_GetAllBookingsForClients @PageNumber, @PageSize, @ClientId", pageNumParam, pageSizeParam, clientId);

            return services.Any() ? services : new List<BookingListDto>();
        }
       
        public ClientStatResponse GetClientStats(ClientStatsRequest request)
        {
            // Get provider by email
            var client = _unitOfWork.GetRepository<Users>()
                .FirstOrDefult(u => u.Email == request.Email);

            if (client == null)
                return new ClientStatResponse(); // return default object instead of null

            var clientId = client.UserId;

            // Get repositories
            var servicesRepo = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>();
            
            var bookingsRepo = _unitOfWork.GetRepository<Bookings>();

            // Materialize data first (avoid multiple open readers)
            var services = servicesRepo.GetAll()
                .ToList().Count();

           
            var bookings = bookingsRepo.GetAll().Where(x => x.ClientId == clientId).ToList().Count();

            // Compose response
            return new ClientStatResponse
            {
                ActiveServices = services,
                BookedServices = bookings
            };
        }

        public int UpdatePaymentStatus(long bookingId, string status)
        {
            var booking = _unitOfWork.GetRepository<Bookings>()
                .FirstOrDefult(b => b.BookingId == bookingId);

            if (booking == null)
                return 0;

            booking.Status = status;
            booking.BookingDate = DateTime.UtcNow;

            _unitOfWork.Commit();
            return 1;
        }

        public List<ServiceResponse> GetAllServicesForClient(AllServicesRequest request)
        {
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var userId = new SqlParameter("@UserId", SqlDbType.Int) { Value = (object?)request.UserId ?? DBNull.Value };
            var services = _unitOfWork.SpListRepository<ServiceResponse>(
           "sp_GetAllServicesForClient @PageNumber, @PageSize, @UserId", pageNumParam, pageSizeParam, userId);

            return services.Any() ? services : new List<ServiceResponse>();
        }
    }


}
