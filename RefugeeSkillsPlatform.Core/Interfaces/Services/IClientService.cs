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
        bool CreateBooking(BookingDTO request);
        int CreatePayment(PaymentDto request);
    }
}
