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
    public class BrandController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        public BrandController(CafeManagerDBContext context)
        {
            _context = context;
        }

        // GET: api/Brand
        [HttpGet]
        public ActionResult<IEnumerable<Brand>> GetBrands()
        {
            return _context.Brands.ToList();
        }

        // GET: api/Brand/id
        [HttpGet("{id}")]
        public ActionResult<Brand> GetBrand(string id)
        {
            var brand = _context.Brands.Find(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        [HttpGet("GetMaxBrandId")]
        public IActionResult GetMaxBrandId()
        {
            var allBrandIds = _context.Brands.Select(p => int.Parse(p.Id)).ToList();

            var maxId = allBrandIds.Any() ? allBrandIds.Max() : 0;

            return Ok(maxId);
        }

        // POST: api/Brand
        [HttpPost]
        public ActionResult<Brand> PostBrand([FromBody] BrandModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brand = new Brand
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };

            _context.Brands.Add(brand);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
        }


        // PUT: api/Brand/id
        [HttpPut("{id}")]
        public IActionResult PutBrand(string id, [FromBody] BrandModel request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingBrand = _context.Brands.Find(id);

            if (existingBrand == null)
            {
                return NotFound();
            }

            existingBrand.Id = request.Id;
            existingBrand.Name = request.Name;
            existingBrand.Description = request.Description;

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

        // DELETE: api/Brand/id
        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(string id)
        {
            var brand = _context.Brands.Find(id);

            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            _context.SaveChanges();

            return NoContent();
        }

        private bool BrandExists(string id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
