using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        List<CategoryResponse> GetCategories();
      
    }
}
