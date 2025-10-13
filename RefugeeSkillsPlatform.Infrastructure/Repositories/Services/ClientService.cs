using Microsoft.Data.SqlClient;
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
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CreateBooking(BookingDTO request)
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
                return false; // booking conflict

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

            return true;
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


        public int CreatePayment(PaymentDto request)
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
            return _unitOfWork.Commit();
        }

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

    }
}
