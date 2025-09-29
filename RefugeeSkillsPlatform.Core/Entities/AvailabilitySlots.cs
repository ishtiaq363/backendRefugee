using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RefugeeSkillsPlatform.Core.Entities
{
    public class AvailabilitySlots
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AvailabilitySlotId { get; set; }

        [Required]
        [ForeignKey("ServiceId")]
        public long ServiceId { get; set; }

        [Required]
        public DateTime SlotStartDate { get; set; } = DateTime.Now;

        public DateTime SlotEndDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

    }
}
