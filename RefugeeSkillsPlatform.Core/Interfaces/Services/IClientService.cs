using RefugeeSkillsPlatform.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces.Services
{
    public interface IClientService
    {
        List<ServiceSlotResponse> GetServiceSlots(ServiceSlotRequest request);
        //bool CreateBooking(BookingDTO request);
        Task<int> CreatePaymentAsync(PaymentDto request);
        List<BookedSlotResponse> GetBookedSlots(int availabilitySlotId, DateTime date);
        List<BookingListDto> GetBookingListForClientId(BookingRequestForClient request);
        ClientStatResponse GetClientStats(ClientStatsRequest request);
        long CreateBooking(BookingDTO request);
        int UpdatePaymentStatus(long bookingId, string status);
        List<ServiceResponse> GetAllServicesForClient(AllServicesRequest request);
    }
}
