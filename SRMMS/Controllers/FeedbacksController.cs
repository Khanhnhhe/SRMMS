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

        // GET: api/Feedbacks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedbacks()
        {
            if (_context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedbacks = await _context.Feedbacks
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    Feedback1 = f.Feedback1,
                    RateStar = f.RateStar,
                    AccId = f.AccId,
                    AccountFullName = f.Acc != null ? f.Acc.FullName : null, // Lấy FullName từ Account
                    CreatedAt = f.CreatedAt.HasValue ? f.CreatedAt.Value.ToString("dd/MM/yyyy") : null,
                    UpdatedAt = f.UpdatedAt.HasValue ? f.UpdatedAt.Value.ToString("dd/MM/yyyy") : null

                })
                .ToListAsync();

            return Ok(feedbacks);
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
                return NotFound(new { Message = "Account not found." });
            }
            var feedback = new Feedback
            {
                Feedback1 = feedbackDto.Feedback1,
                RateStar = feedbackDto.RateStar,
                AccId = feedbackDto.AccId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

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
                RateStar = feedback.RateStar,
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

        private bool FeedbackExists(int id)
        {
            return (_context.Feedbacks?.Any(e => e.FeedbackId == id)).GetValueOrDefault();
        }
    }
}
