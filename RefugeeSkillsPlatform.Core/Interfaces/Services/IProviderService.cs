using RefugeeSkillsPlatform.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces.Services
{
    public interface IProviderService
    {
        int CreateSerice(ServiceRequest request);
        List<ServiceResponse> GetAllServices(AllServicesRequest request);
        List<BookingListDto> GetBookingList(AllBookingsRequest request);
        int CreateSlots(AvailabilitySlotsDTO request);

        bool IsAccountApproved(int providerId);

    }
}
