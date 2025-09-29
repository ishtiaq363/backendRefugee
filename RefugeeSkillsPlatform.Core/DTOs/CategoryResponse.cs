using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.DTOs
{
    public class CategoryResponse
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Logo { get; set; }

    }
}
