using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeApp.Models;
using CafeApp.AppModels;

namespace CafeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        public ProductController(CafeManagerDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    BrandName = p.Brand.Name,
                    CategoryName = p.Category.Name
                })
                .ToList();

            return Ok(products);
        }

        //[HttpGet("{id}")]
        //public IActionResult GetProduct(string id)
        //{
        //    var product = _context.Products.Find(id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(product);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetails>> GetProductById(string id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDetailsDto = new ProductDetails
            {
                ProductName = product.Name,
                ProductDescription = product.Description,
                ProductImageUrl = product.ImageUrl,
                BrandName = product.Brand?.Name,
                BrandDescription = product.Brand?.Description,
                CategoryName = product.Category?.Name,
                CategoryDescription = product.Category?.Description
            };

            return Ok(productDetailsDto);
        }

        [HttpGet("GetMaxProductId")]
        public IActionResult GetMaxProductId()
        {
            var allProductIds = _context.Products.Select(p => int.Parse(p.Id)).ToList();

            var maxId = allProductIds.Any() ? allProductIds.Max() : 0;

            return Ok(maxId);
        }


        [HttpPost]
        public IActionResult PostProduct([FromBody] ProductModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brandId = _context.Brands.FirstOrDefault(e => e.Name == request.BrandName);
            var categoryId = _context.Categories.FirstOrDefault(e => e.Name == request.CategoryName);

            if (brandId == null || categoryId == null) {
                return BadRequest();
            }

            var product = new Product
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                BrandId = brandId.Id,
                CategoryId = categoryId.Id
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            var productDto = new ProductModel
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                BrandName = request.BrandName,
                CategoryName = request.CategoryName
            };

            return CreatedAtAction("GetProduct", new { id = productDto.Id }, productDto);
        }


        [HttpPut("{id}")]
        public IActionResult PutProduct(string id, [FromBody] ProductModel request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingProduct = _context.Products.Find(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            var brandId = _context.Brands.FirstOrDefault(e => e.Name == request.BrandName);
            var categoryId = _context.Categories.FirstOrDefault(e => e.Name == request.CategoryName);

            if (brandId == null || categoryId == null)
            {
                return BadRequest();
            }

            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.ImageUrl = request.ImageUrl;
            existingProduct.BrandId = brandId.Id;
            existingProduct.CategoryId = categoryId.Id;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return StatusCode(500, "Concurrency error occurred while updating the product.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(string id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(product);
        }
    }
}
