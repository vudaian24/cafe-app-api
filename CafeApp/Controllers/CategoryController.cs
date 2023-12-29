using System;
using System.Collections.Generic;
using System.Linq;
using CafeApp.AppModels;
using CafeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        public CategoryController(CafeManagerDBContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            return _context.Categories.ToList();
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(string id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpGet("GetMaxCategoryId")]
        public IActionResult GetMaxCategoryId()
        {
            var allCategoryIds = _context.Categories.Select(p => int.Parse(p.Id)).ToList();

            var maxId = allCategoryIds.Any() ? allCategoryIds.Max() : 0;

            return Ok(maxId);
        }

        // POST: api/Category
        [HttpPost]
        public ActionResult<Category> PostCategory([FromBody] CategoryModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category()
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // PUT: api/Category/id
        [HttpPut("{id}")]
        public IActionResult PutCategory(string id, [FromBody] CategoryModel request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingCategory = _context.Categories.Find(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Id = request.Id;
            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description;
            
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return StatusCode(500, "Concurrency error occurred while updating the brand.");
            }

            return NoContent();
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(string id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }

        private bool CategoryExists(string id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
