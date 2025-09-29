using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Entities
{
    public class ServiceDeliveryOptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ServiceDeliveryOptionId { get; set; }
        public long UserId { get; set; }
        public long ServiceId { get; set; }
        public long DeliveryMethodId { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
