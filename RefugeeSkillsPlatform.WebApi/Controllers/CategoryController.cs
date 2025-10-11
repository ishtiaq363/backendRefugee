using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.WebApi.Common;

namespace RefugeeSkillsPlatform.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase    
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categoriesResponse = _categoryService.GetCategories();
            if (categoriesResponse == null)
            {
                return Ok(new StandardRequestResponse<List<CategoryResponse>>() { Data = null, Success = false, Message = "Internal Server Error", Status = 500 }
                    );
            }
            return Ok(new StandardRequestResponse<List<CategoryResponse>>()
            {
                Data = categoriesResponse,
                Message = "Successfully fetch methods",
                Success = true,
                Status = 200
            });

            //return Ok(_categoryService.GetCategories());
        }

    }
}
