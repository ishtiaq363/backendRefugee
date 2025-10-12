using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class PaymentDto
    {
      public long PaymentId { get; set; }
      public long BookingId { get; set; }
      public decimal Amount { get; set; }
      public DateTime PaymentDate { get; set; }
      public string PaymentStatus { get; set; }
      public string TransactionReference { get; set; }
      public string PaymentMethod { get; set; }
      public bool? IsRefunded { get; set; }
      public decimal? RefundAmount { get; set; }
      public DateTime? RefundDate { get; set; }
      public string? RefundReference { get; set; }
      public string? RefundReason { get; set; }
    }

    public class BookedSlotResponse
    {
        public TimeSpan BookingStart { get; set; }
        public TimeSpan BookingEnd { get; set; }
    }
}
