using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class BookingListDto
    {
        public long BookingId { get; set; }
        public long AvailabilitySlotId { get; set; }
        public long ClientId { get; set; }
        public TimeSpan BookingStart { get; set; }
        public TimeSpan BookingEnd { get; set; }
        public DateTime SlotStartDate { get; set; }
        public DateTime SlotEndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string? ZoomLink { get; set; } 
        public string Client { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public long RowNum { get; set; }
        public string? PaymentStatus { get; set; } = string.Empty;
    }


}
