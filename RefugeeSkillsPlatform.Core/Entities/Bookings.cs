using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Entities
{
    public class Bookings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BookingId { get; set; }
        [Required]
        public long AvailabilitySlotId { get; set; }
        [Required]
        public long ClientId { get; set; }
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

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

    }
}
