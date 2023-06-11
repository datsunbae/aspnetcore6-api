using api_aspnetcore6.Dtos;
using api_aspnetcore6.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_aspnetcore6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var response = await _productService.GetAllProducts(pagingParameters);
                _logger.LogInformation($"SeriLog - Product (GetAllProducts): Get all products successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Product (GetAllProducts): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchProduct(string? name, double? priceFrom, double? priceTo, int? idCategory,  [FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var response = await _productService.SearchProduct(name, priceFrom, priceTo, idCategory, pagingParameters);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Product (GetAllProducts): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var response = await _productService.GetProductById(id);

                if (response == null)
                {
                    _logger.LogWarning($"SeriLog - Product (GetProductById): Product not found");
                    return NotFound("Product not found");
                }

                _logger.LogInformation($"SeriLog - Product (GetProductById): Product found (Name: {response.Name})");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Product (GetProductById): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"SeriLog - Product (AddProduct): Product not found");
                    return BadRequest();
                }

                var response = await _productService.AddProduct(product);
                _logger.LogInformation($"SeriLog - Product (AddProduct): Add product successfully (Id: {product.Id})");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Product (AddProduct): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO product)
        {
            try
            {
                if (!ModelState.IsValid || id == null)
                {
                    _logger.LogWarning($"SeriLog - Product (UpdateProduct): Product not found");
                    return BadRequest();
                }

                var reponse = await _productService.UpdateProduct(id, product);

                _logger.LogInformation($"SeriLog - Product (UpdateProduct): Update product (Id: {id}) successfully");
                return Ok(reponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Product (UpdateProduct): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning($"SeriLog - Product (DeleteProduct): Product not found");
                    return BadRequest();
                }

                var response = await _productService.DeleteProduct(id);

                _logger.LogInformation($"SeriLog - Product (DeleteProduct): Deleted product (ID : {id}) successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SeriLog - Product (DeleteProduct): An internal server error occurred {ex.Message}");
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }
    }
}