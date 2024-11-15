using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Hubs;
using SRMMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly SRMMSContext _context;
        private readonly IHubContext<FeedbackHub> _hubContext;

        public FeedbackController(SRMMSContext context, IHubContext<FeedbackHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("/api/feedback/getList")]
        public async Task<IActionResult> GetFeedbackList()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Acc)
                .Select(f => new
                {
                    f.FeedbackId,
                    f.Feedback1,
                    f.RateStar,
                    f.CreatedAt,
                    f.UpdatedAt,
                    AccountName = f.Acc != null ? f.Acc.FullName : null
                })
                .ToListAsync();

            return Ok(feedbacks);
        }

        [HttpPost("/api/feedback/create")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO feedbackDto)
        {
            if (feedbackDto == null)
            {
                return BadRequest("Invalid feedback data.");
            }

            var feedback = new Feedback
            {
                Feedback1 = feedbackDto.Feedback1,
                RateStar = feedbackDto.RateStar,
                AccId = feedbackDto.AccId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var feedbacks = await _context.Feedbacks
                .Include(f => f.Acc)
                .Select(f => new
                {
                    f.FeedbackId,
                    f.Feedback1,
                    f.RateStar,
                    f.CreatedAt,
                    f.UpdatedAt,
                    AccountName = f.Acc != null ? f.Acc.FullName : null
                })
                .ToListAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveFeedbackUpdate", feedbacks);

            return CreatedAtAction(nameof(CreateFeedback), new { id = feedback.FeedbackId }, feedback);
        }

        
        
    }
}
