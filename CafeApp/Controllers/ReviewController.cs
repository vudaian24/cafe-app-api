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
    public class ReviewController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        public ReviewController(CafeManagerDBContext context)
        {
            _context = context;
        }

        // GET: api/Review
        [HttpGet]
        public ActionResult<IEnumerable<Review>> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        [HttpGet("GetMaxReviewId")]
        public IActionResult GetMaxReviewId()
        {
            var allIds = _context.Reviews.Select(p => int.Parse(p.Id)).ToList();

            var maxId = allIds.Any() ? allIds.Max() : 0;

            return Ok(maxId);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult> GetReviewsByProductId(string productId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.ProductId == productId)
                    .ToListAsync();

                var userComments = reviews.Select(r => new
                {
                    UserName = _context.Users.FirstOrDefault(u => u.Id == r.UserId)?.Name,
                    Avatar = _context.Users.FirstOrDefault(u => u.Id == r.UserId)?.Avatar,
                    Comment = r.Comment
                }).ToList();

                return Ok(userComments);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        //POST: api/Review
        [HttpPost]
        public ActionResult<Review> PostReview([FromBody] ReviewModel result)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var review = new Review
            {
                Id = result.id,
                Comment = result.comment,
                ProductId = result.productId,
                UserId = result.userId
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();

            return Ok(review);
        }

        // PUT: api/Review/id
        [HttpPut("{id}")]
        public IActionResult PutReview(string id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Review/id
        [HttpDelete("{id}")]
        public IActionResult DeleteReview(string id)
        {
            var review = _context.Reviews.Find(id);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return NoContent();
        }

        private bool ReviewExists(string id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
