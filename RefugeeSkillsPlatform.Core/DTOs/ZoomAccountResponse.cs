using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class ZoomAccountResponse
    {
        public long UserId { get; set; }
        public string ZoomEmail { get; set; }
        public DateTime TokenExpiresAt { get; set; }
    }
}
