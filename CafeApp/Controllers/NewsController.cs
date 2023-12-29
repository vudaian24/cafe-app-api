using System;
using System.Collections.Generic;
using System.Linq;
using CafeApp.AppModels;
using CafeApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        public NewController(CafeManagerDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<New>> GetNews()
        {
            return _context.News.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<New> GetNew(string id)
        {
            var news = _context.News.Find(id);

            if (news == null)
            {
                return NotFound();
            }

            return news;
        }

        [HttpGet("GetMaxNewId")]
        public IActionResult GetMaxNewId()
        {
            var maxId = _context.News.Max(news => news.Id);

            return Ok(maxId);
        }


        [HttpPost]
        public ActionResult<New> PostNew([FromBody] NewsModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var news = new New
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl
                // Add other properties as needed
            };

            _context.News.Add(news);
            _context.SaveChanges();

            return CreatedAtAction("GetNew", new { id = news.Id }, news);
        }

        [HttpPut("{id}")]
        public IActionResult PutNew(string id, [FromBody] NewsModel request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingNews = _context.News.Find(id);

            if (existingNews == null)
            {
                return NotFound();
            }

            existingNews.Id = request.Id;
            existingNews.Title = request.Title;
            existingNews.Description = request.Description;
            existingNews.ImageUrl = request.ImageUrl;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Concurrency error occurred while updating the news.");
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public ActionResult<New> DeleteNew(string id)
        {
            var news = _context.News.Find(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);
            _context.SaveChanges();

            return news;
        }
    }
}
