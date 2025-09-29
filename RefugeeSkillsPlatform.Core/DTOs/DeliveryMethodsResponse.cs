using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class DeliveryMethodsResponse
    {
        public long DeliveryMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string? Description { get; set; }

    }
}
