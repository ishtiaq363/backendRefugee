using Azure.Core;
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
    public class ProviderService : IProviderService
    {
       private readonly IUnitOfWork _unitOfWork;
        public ProviderService(IUnitOfWork unitOfWrok)
        {
            _unitOfWork = unitOfWrok;
        }
        public int CreateSerice(ServiceRequest request)
        {
            
            var service = new RefugeeSkillsPlatform.Core.Entities.Services()
            {
                ServiceName = request.ServiceName,
                Description = request.Description,
                CategoryId = request.CategoryId,
                CreatedByUserId = request.CreatedByUserId,
                DeliveryMethodId = request.DeliveryMethodId,
                IsApproved = false,
                CreatedOn = DateTime.UtcNow,
                FeeCharges = request.FeeCharges ?? 0,
                IsScheduled = false
            };
            if(service is null)
            {
                return 0;
            }
            _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>().Add(service);
            var result = _unitOfWork.Commit();
            return result;
        }
        public int CreateSlots(AvailabilitySlotsDTO request)
        {
            var slot = new AvailabilitySlots() {
            ServiceId = request.ServiceId,
            SlotStartDate = request.SlotStartDate,
            SlotEndDate = request.SlotEndDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime
            };
            if(slot is null)
            {
                return 0;
            }
            _unitOfWork.GetRepository<AvailabilitySlots>().Add(slot);
            var service = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>().FirstOrDefult(s => s.ServiceId == request.ServiceId);
            if(service != null)
            {
                service.IsScheduled = true; // Mark service as unapproved when new slots are added
                _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>().Update(service);
            }
            var result = _unitOfWork.Commit();
            return result;

        }

        public List<ServiceResponse> GetAllServicesForProvider(AllServicesForProviderRequest request)
        {
            var provider = _unitOfWork.GetRepository<Users>().FirstOrDefult(u => u.Email == request.Email);
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var userId = new SqlParameter("@UserId", SqlDbType.Int) { Value = (object?)provider.UserId ?? DBNull.Value };
            var services = _unitOfWork.SpListRepository<ServiceResponse>(
           "sp_GetAllServices @PageNumber, @PageSize, @UserId", pageNumParam, pageSizeParam, userId);

            return services.Any() ? services : new List<ServiceResponse>();
        }

        public ProviderStatResponse GetProviderStats(ProviderStatsRequest request)
        {
            // Get provider by email
            var provider = _unitOfWork.GetRepository<Users>()
                .FirstOrDefult(u => u.Email == request.Email);

            if (provider == null)
                return new ProviderStatResponse(); // return default object instead of null

            var providerId = provider.UserId;

            // Get repositories
            var servicesRepo = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>();
            var slotsRepo = _unitOfWork.GetRepository<AvailabilitySlots>();
            var bookingsRepo = _unitOfWork.GetRepository<Bookings>();

            // Materialize data first (avoid multiple open readers)
            var services = servicesRepo.GetAll()
                .Where(s => s.CreatedByUserId == providerId)
                .ToList();

            var slots = slotsRepo.GetAll().ToList();
            var bookings = bookingsRepo.GetAll().ToList();

            // Active & pending services
            var activeServices = services.Count(s => s.IsApproved);
            var serviceRequests = services.Count(s => !s.IsApproved);

            // Booked services
            var bookedServices = (
                from booking in bookings
                join slot in slots on booking.AvailabilitySlotId equals slot.AvailabilitySlotId
                join service in services on slot.ServiceId equals service.ServiceId
                select booking
            ).Count();

            // Unique clients
            var totalClients = (
                from booking in bookings
                join slot in slots on booking.AvailabilitySlotId equals slot.AvailabilitySlotId
                join service in services on slot.ServiceId equals service.ServiceId
                select booking.ClientId
            ).Distinct().Count();

            // Compose response
            return new ProviderStatResponse
            {
                ActiveServices = activeServices,
                ServiceResquests = serviceRequests,
                BookedServices = bookedServices,
                TotalClients = totalClients
            };
        }


        public AdminStatResponse GetAdminStats()
        {            
            var totalProvider =  _unitOfWork.GetRepository<Users>().GetAll().Where(x => x.RoleId == 4002).Count();
            var totalClients = _unitOfWork.GetRepository<Users>().GetAll().Where(x => x.RoleId == 4001).Count();
            var activeService = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>().GetAll().Where(x => x.IsApproved == true).Count();
            var serviceRequest = _unitOfWork.GetRepository<RefugeeSkillsPlatform.Core.Entities.Services>().GetAll().Where(x => x.IsApproved == false).Count();
            var bookedServices = _unitOfWork.GetRepository<Bookings>().GetAll().Count();
            AdminStatResponse adminStats = new AdminStatResponse() { 
            TotalProviders = totalProvider,
            TotalClients = totalClients,
            ActiveServices = activeService,
            BookedServices = bookedServices,
            ServiceResquests = serviceRequest
            };
            return adminStats;
        }

        public List<ServiceResponse> GetAllServices(AllServicesRequest request)
        {
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var userId = new SqlParameter("@UserId", SqlDbType.Int) { Value = (object?)request.UserId ?? DBNull.Value };
            var services = _unitOfWork.SpListRepository<ServiceResponse>(
           "sp_GetAllServices @PageNumber, @PageSize, @UserId", pageNumParam, pageSizeParam,userId);

            return services.Any() ? services : new List<ServiceResponse>();
        }

        public List<BookingListDto> GetBookingList(AllBookingsRequest request)
        {
            var pageNumParam = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = request.PageNumber };
            var pageSizeParam = new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.PageSize };
            var bookingId = new SqlParameter("@BookingId", SqlDbType.BigInt) { Value = (object?)request.BookingId ?? DBNull.Value };
            var services = _unitOfWork.SpListRepository<BookingListDto>(
           "Sp_GetAllBookings @PageNumber, @PageSize, @BookingId", pageNumParam, pageSizeParam, bookingId);

            return services.Any() ? services : new List<BookingListDto>();
        }

        public bool IsAccountApproved(int providerId)
        {
            var provider = _unitOfWork.GetRepository<Users>().FirstOrDefult(u => u.UserId == providerId);
            if (provider != null)
            {
                return provider.IsApproved;
            }
            return false;
        }
    }
}
