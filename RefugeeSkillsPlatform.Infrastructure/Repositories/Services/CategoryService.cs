using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Repositories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<CategoryResponse> GetCategories()
        {
            try
            {
                var categoryList = _unitOfWork.GetRepository<Category>().GetAll();
                return categoryList.Select(category => new CategoryResponse()
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Logo = category.Logo,
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<CategoryResponse>();
            }
        }

    }
}
