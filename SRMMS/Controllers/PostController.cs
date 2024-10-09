using Microsoft.AspNetCore.Mvc;
using SRMMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using System;

namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public PostController(SRMMSContext context)
        {
            _context = context;
        }

        
        [HttpGet("ShowListPost")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.EmpPost) 
                .Include(p => p.PostToppic) 
                .Select(p => new PostDTO
                {
                    PostId = p.PostId,
                    PostTitle = p.PostTitle,
                    EmpName = p.EmpPost.EmpFirstName + " " + p.EmpPost.EmpLastName, 
                    PostToppicId = p.PostToppicId,
                    PostImg = p.PostImg,
                    PostDetail = p.PostDetail,
                    PostDate = p.PostDate,
                    PostToppicName = p.PostToppic.TopicName 
                })
                .ToListAsync();

            return Ok(posts);
        }



        [HttpPost("AddPost")]
        public async Task<ActionResult<AddPostDTO>> AddPost(AddPostDTO postDTO)
        {
            if (postDTO == null)
            {
                return BadRequest("Post data is null.");
            }
            var post = new Post
            {
                PostTitle = postDTO.PostTitle,
                EmpPostId = postDTO.EmpPostId,
                PostToppicId = postDTO.PostToppicId,
                PostImg = postDTO.PostImg,
                PostDetail = postDTO.PostDetail,
                PostDate = DateTime.UtcNow
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();  
            return CreatedAtAction(nameof(GetPosts), new { id = post.PostId }, postDTO);
        }


        
        [HttpPut("UpdatePost/{id}")]
        public async Task<ActionResult> UpdatePost(int id, AddPostDTO postDTO)
        {
            if (id != postDTO.PostId)
            {
                return BadRequest("Post ID mismatch.");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(postDTO.PostTitle))
            {
                post.PostTitle = postDTO.PostTitle;
            }

            if (postDTO.EmpPostId != default(int))
            {
                post.EmpPostId = postDTO.EmpPostId;
            }

            if (postDTO.PostToppicId != default(int))
            {
                post.PostToppicId = postDTO.PostToppicId;
            }

            if (!string.IsNullOrWhiteSpace(postDTO.PostImg))
            {
                post.PostImg = postDTO.PostImg;
            }

            if (!string.IsNullOrWhiteSpace(postDTO.PostDetail))
            {
                post.PostDetail = postDTO.PostDetail;
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }



       
        [HttpDelete("DeletePost/{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }




}
