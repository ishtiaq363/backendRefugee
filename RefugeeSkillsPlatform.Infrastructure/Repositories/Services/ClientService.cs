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
            // Check if any booking already exists for the same slot and overlapping times
            //var isAlreadyBooked = _unitOfWork.GetRepository<Bookings>().GetAll().ToList()
            //    .Any(x => x.AvailabilitySlotId == request.AvailabilitySlotId &&
            //              x.SlotStartDate == request.SlotStartDate &&
            //              x.SlotEndDate == request.SlotEndDate &&
            //              (
            //                  (request.BookingStart >= x.BookingStart && request.BookingStart < x.BookingEnd) ||
            //                  (request.BookingEnd > x.BookingStart && request.BookingEnd <= x.BookingEnd) ||
            //                  (request.BookingStart <= x.BookingStart && request.BookingEnd >= x.BookingEnd) // new booking fully overlaps existing
            //              )
            //    );
                        var isAlreadyBooked = _unitOfWork.GetRepository<Bookings>().GetAll().Any(x =>
                x.AvailabilitySlotId == request.AvailabilitySlotId &&
                x.SlotStartDate.Date == request.SlotStartDate.Date &&
                x.SlotEndDate.Date == request.SlotEndDate.Date &&
                (
                    (request.BookingStart >= x.BookingStart && request.BookingStart < x.BookingEnd) ||
                    (request.BookingEnd > x.BookingStart && request.BookingEnd <= x.BookingEnd) ||
                    (request.BookingStart <= x.BookingStart && request.BookingEnd >= x.BookingEnd) ||
                    (x.BookingStart == x.BookingEnd && request.BookingStart == x.BookingStart) // exact match case
                )
            );



            if (isAlreadyBooked)
                return false; // booking conflict

            // If not already booked, save the booking
            var newBooking = new Bookings
            {
                AvailabilitySlotId = request.AvailabilitySlotId,
                ClientId = request.ClientId,
                BookingStart = request.BookingStart,
                BookingEnd = request.BookingEnd,
                SlotStartDate = request.SlotStartDate,
                SlotEndDate = request.SlotEndDate,
                Status = request.Status,
                BookingDate = request.BookingDate
            };

            _unitOfWork.GetRepository<Bookings>().Add(newBooking);
            _unitOfWork.Commit();

            return true;
        }


        public List<ServiceSlotResponse> GetServiceSlots(ServiceSlotRequest request)
        {
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            // var userId = new SqlParameter("@UserId", SqlDbType.Int) { Value = (object?)request.UserId ?? DBNull.Value };
            var services = _unitOfWork.SpListRepository<ServiceSlotResponse>(
           "sp_GetAllServiceSlots @PageNumber, @PageSize", pageNumParam, pageSizeParam);

            return services.Any() ? services : new List<ServiceSlotResponse>();
        }
    }
}
