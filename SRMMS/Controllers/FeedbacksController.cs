using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public FeedbacksController(SRMMSContext context)
        {
            _context = context;
        }
        //check 
        // GET: api/Feedbacks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedbacks(
      int? rateStar = null,
      int pageNumber = 1,
      int pageSize = 10)
        {
            if (_context.Feedbacks == null)
            {
                return NotFound();
            }

      
            var feedbackQuery = _context.Feedbacks.AsQueryable();

          
            if (rateStar.HasValue)
            {
                feedbackQuery = feedbackQuery.Where(f => f.RateStar == rateStar.Value);
            }

            var totalItems = await feedbackQuery.CountAsync(); 
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); 

            
            var feedbacks = await feedbackQuery
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize) 
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    Feedback1 = f.Feedback1,
                    RateStar = f.RateStar,
                    AccId = f.AccId,
                    AccountFullName = f.Acc != null ? f.Acc.FullName : null,
                    CreatedAt = f.CreatedAt.HasValue ? f.CreatedAt.Value.ToString("dd/MM/yyyy") : null,
                    UpdatedAt = f.UpdatedAt.HasValue ? f.UpdatedAt.Value.ToString("dd/MM/yyyy") : null
                })
                .ToListAsync();

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Feedbacks = feedbacks
            };

            return Ok(response);
        }

        // GET: api/Feedbacks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
            if (_context.Feedbacks == null)
            {
                return NotFound();
            }
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }

     

        // POST: api/Feedbacks
        [HttpPost]
        public async Task<ActionResult<FeedbackResponseDto>> PostFeedback(FeedbackRequestDto feedbackDto)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set 'SRMMSContext.Feedbacks' is null.");
            }
            var account = await _context.Accounts.FindAsync(feedbackDto.AccId);
            if (account == null)
            {
                return BadRequest(new { Message = "Account not found." });
            }
            if (feedbackDto.RateStar < 1 || feedbackDto.RateStar > 5)
            {
                return BadRequest(new { Message = "RateStar must be between 1 and 5." });
            }
            if (_context.Feedbacks == null)
            {
                return StatusCode(500, new { Message = "Feedback storage is unavailable." });
            }
            var feedback = new Feedback
            {
                Feedback1 = feedbackDto.Feedback1,
                RateStar = feedbackDto.RateStar,
                AccId = feedbackDto.AccId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            feedback.Acc = account;
            try
            {
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { Message = "An error occurred while saving feedback.", Details = ex.InnerException?.Message });
            }

            var feedbackResponse = new FeedbackResponseDto
            {
                FeedbackId = feedback.FeedbackId,
                Feedback1 = feedback.Feedback1,
                RateStar = (int)feedback.RateStar,
                FullName = feedback.Acc?.FullName ?? "Anonymous",
                CreatedAt = feedback.CreatedAt?.ToString("dd/MM/yyyy")
            };

            return CreatedAtAction("GetFeedback", new { id = feedback.FeedbackId }, feedbackResponse);
        }

        // DELETE: api/Feedbacks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            if (_context.Feedbacks == null)
            {
                return NotFound();
            }
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

     
    }
}
