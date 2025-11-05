using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefugeeSkillsPlatform.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Security.Principal;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public  class ServiceRequest
    {
        public long? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public long CreatedByUserId { get; set; }
        public long DeliveryMethodId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? FeeCharges { get; set; }
        public bool? IsScheduled { get; set; }
    }

    public class AdminStatResponse
    {
        public int ActiveServices { get; set; } = 0;
        public int TotalClients { get; set; } = 0;
        public int TotalProviders { get; set; } = 0;
        public int ServiceResquests { get; set; } = 0;
        public int BookedServices { get; set; } = 0;
       
    }

    public class ProviderStatResponse
    {
        public int ActiveServices { get; set; } = 0;
        public int TotalClients { get; set; } = 0;
        public int ServiceResquests { get; set; } = 0;
        public int BookedServices { get; set; } = 0;
    }

    public class AllServicesRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Add this for filtering by user
        public int? UserId { get; set; }
    }
    public class AllServicesForProviderRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Add this for filtering by user
        public string? Email { get; set; }
    }
    public class AllBookingsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? BookingId { get; set; }
    }
    public class ServiceResponse
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CreatedByUserId { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public long DeliveryMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; }
        public bool? IsApproved { get; set; }
        public long RowNum { get; set; }   // comes from ROW_NUMBER()
        public string? Logo { get; set; }

        public decimal FeeCharges { get; set; }
        public bool? IsScheduled { get; set; }

        public DateTime? SlotEndDate { get; set; }
        public bool? isExpired { get; set; }
        public int daysRemaining { get; set; }

    }
    
    public class AvailabilitySlotsDTO
    {
        public long AvailabilitySlotId { get; set; }
        public long ServiceId { get; set; }
        public DateTime SlotStartDate { get; set; } = DateTime.Now;
        public DateTime SlotEndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
     }       
    public class ProviderStatsRequest
    {
        public string? Email { get; set; }
    }
    public class ClientStatsRequest
    {
        public string? Email { get; set; }
    }
    public class ClientStatResponse
    {
        public int ActiveServices { get; set; } = 0;
        public int BookedServices { get; set; } = 0;
    }
}
