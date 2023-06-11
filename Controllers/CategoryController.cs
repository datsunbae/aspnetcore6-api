using api_aspnetcore6.Dtos;
using api_aspnetcore6.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_aspnetcore6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var response = await _categoryService.GetALlCategories(pagingParameters);
                _logger.LogInformation($"SeriLog - Category (GetAllCategories): Get all categories successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Category (GetAllCategories): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var response = await _categoryService.GetCategoryById(id);

                if (response == null)
                {
                    _logger.LogWarning($"SeriLog - Category (GetCategoryById): Category not found");
                    return NotFound("Category not found");
                }

                _logger.LogInformation($"SeriLog - Category (GetCategoryById): Category found (Name: {response.Name})");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Category (GetCategoryById): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCategory(string name, [FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                return Ok(await _categoryService.SearchCategories(name, pagingParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Category (GetCategoryById): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"SeriLog - Category (AddCategory): Category not found");
                    return BadRequest();
                }

                var response = await _categoryService.AddCategory(category);
                _logger.LogInformation($"SeriLog - Category (AddCategory): Add category successfully (Id: {category.Id})");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Category (AddCategory): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDTO category)
        {
            try
            {
                if (!ModelState.IsValid || id == null)
                {
                    _logger.LogWarning($"SeriLog - Category (UpdateCategory): Category not found");
                    return BadRequest();
                }

                var reponse = await _categoryService.UpdateCategory(id, category);

                _logger.LogInformation($"SeriLog - Category (UpdateCategory): Update category (Id: {id}) successfully");
                return Ok(reponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Category (UpdateCategory): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning($"SeriLog - Category (DeleteCategory): Category not found");
                    return BadRequest();
                }

                var response = await _categoryService.DeleteCategory(id);

                _logger.LogInformation($"SeriLog - Category (DeleteCategory): Deleted category (ID : {id}) successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Category (DeleteCategory): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }
    }
}