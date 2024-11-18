using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly SRMMSContext _context;
        private readonly IHubContext<FeedbackHub> _hubContext;
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
                .Include(f => f.Acc)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    Feedback1 = f.Feedback1,
                    RateStar = f.RateStar,
                    AccId = f.AccId,
                    CreatedAt = f.CreatedAt,
                    FullName = f.Acc.FullName
                }).ToListAsync();

            return Ok(feedbacks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackDto>> GetFeedback(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Acc)
                .Where(f => f.FeedbackId == id)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    Feedback1 = f.Feedback1,
                    RateStar = f.RateStar,
                    AccId = f.AccId,
                    CreatedAt = f.CreatedAt,
                    FullName = f.Acc.FullName
                }).FirstOrDefaultAsync();

            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }


      
       


        // POST: api/Feedbacks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FeedbackOutputDto>> PostFeedback(FeedbackInputDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Feedback1))
            {
                return BadRequest("Feedback content is required.");
            }

            if (input.RateStar < 1 || input.RateStar > 5)
            {
                return BadRequest("RateStar must be between 1 and 5.");
            }

            // Tìm tài khoản dựa trên Email
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == input.AccountEmail);
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            // Tạo thực thể Feedback mới
            var feedback = new Feedback
            {
                Feedback1 = input.Feedback1,
                RateStar = input.RateStar,
                AccId = account.AccId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

          
            var output = new FeedbackOutputDto
            {
                FeedbackId = feedback.FeedbackId,
                Feedback1 = feedback.Feedback1,
                RateStar = (int)feedback.RateStar,
                AccId = (int)feedback.AccId,
                CreatedAt = (DateTime)feedback.CreatedAt,
                UpdatedAt = (DateTime)feedback.UpdatedAt,
                Acc = new AccountDto
                {
                    AccId = account.AccId,
                    FullName = account.FullName
                }
            };

            return CreatedAtAction(nameof(GetFeedback), new { id = output.FeedbackId }, output);
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
