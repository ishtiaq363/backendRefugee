using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefugeeSkillsPlatform.Core.Interfaces.Services;

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
            return Ok(_categoryService.GetCategories());
        }
        
    }
}
