using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly SRMMSContext _context;
        private readonly IHubContext<FeedbackHub> _hubContext;

        public FeedbacksController(SRMMSContext context, IHubContext<FeedbackHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
                .Select(static f => new FeedbackDto
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

        // PUT: api/Feedbacks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedback(int id, Feedback feedback)
        {
            if (id != feedback.FeedbackId)
            {
                return BadRequest();
            }

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Notify clients about the updated feedback
                await _hubContext.Clients.All.SendAsync("FeedbackUpdated", feedback);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
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

        // POST: api/Feedbacks
        [HttpPost]
        public async Task<ActionResult<FeedbackResponseDto>> PostFeedback(FeedbackRequestDto feedbackDto)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set 'SRMMSContext.Feedbacks' is null.");
            }

            // Tạo mới thực thể Feedback với thông tin từ DTO
            var feedback = new Feedback
            {
                Feedback1 = feedbackDto.Feedback1,
                RateStar = feedbackDto.RateStar,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Map Feedback sang FeedbackResponseDto để trả về kết quả
            var feedbackResponse = new FeedbackResponseDto
            {
                FeedbackId = feedback.FeedbackId,
                Feedback1 = feedback.Feedback1,
                RateStar = feedback.RateStar,
                FullName = feedback.Acc?.FullName ?? "Anonymous",
                CreatedAt = feedback.CreatedAt?.ToString("dd/MM/yyyy")
               
            };

            // Thông báo cho tất cả các client qua SignalR về feedback mới
            await _hubContext.Clients.All.SendAsync("FeedbackCreated", feedbackResponse);

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

            // Notify clients about the deleted feedback
            await _hubContext.Clients.All.SendAsync("FeedbackDeleted", feedback.FeedbackId);

            return NoContent();
        }

        private bool FeedbackExists(int id)
        {
            return (_context.Feedbacks?.Any(e => e.FeedbackId == id)).GetValueOrDefault();
        }
    }
}
