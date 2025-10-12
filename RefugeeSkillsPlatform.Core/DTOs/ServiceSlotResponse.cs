using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class ServiceSlotResponse
    {
        public long AvailabilitySlotId { get; set; }
        public long ServiceId { get; set; }
        public DateTime SlotStartDate { get; set; }
        public DateTime SlotEndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Service Info
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;

        // Category Info
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Logo { get; set; }

        // Delivery Method Info
        public long DeliveryMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string DeliveryMethodDescription { get; set; } = string.Empty;
    }

    public class BookingDTO
    {
        public long? BookingId { get; set; }
        [Required]
        public long AvailabilitySlotId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public TimeSpan BookingStart { get; set; }

        [Required]
        public TimeSpan BookingEnd { get; set; }

        [Required]
        public DateTime SlotStartDate { get; set; }

        [Required]
        public DateTime SlotEndDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
        public string? ZoomLink {get; set;}
        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
    public class ServiceSlotRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long? ServiceId { get; set; }
    }


}
