using RefugeeSkillsPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Entities
{
    public class Services
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public long CreatedByUserId { get; set; }
        public long DeliveryMethodId { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal FeeCharges { get; set; }

        public bool IsScheduled { get; set; }
    }
 

}
