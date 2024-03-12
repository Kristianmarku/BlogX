using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogX.Data;
using BlogX.DTOs;

namespace BlogX.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .ToListAsync();

            if (posts == null || !posts.Any())
            {
                return NotFound();
            }

            var postDtos = posts.Select(p => new PostDto
            {
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CategoryId = p.CategoryId,
                DateCreated = p.DateCreated,
                DateModified = p.DateModified,
                User = new UserDto
                {
                    UserId = p.User?.Id ?? string.Empty,
                    UserName = p.User?.UserName ?? string.Empty,
                    FirstName = p.User?.FirstName ?? string.Empty,
                    LastName = p.User?.LastName ?? string.Empty,
                },
                Comments = p.Comments?.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    DateCreated = c.DateCreated,
                    DateModified = c.DateModified,
                    UserId = c.UserId,
                    PostId = c.PostId,
                    User = new UserDto
                    {
                        UserId = c.User?.Id ?? string.Empty,
                        UserName = c.User?.UserName ?? string.Empty,
                        FirstName = c.User?.FirstName ?? string.Empty,
                        LastName = c.User?.LastName ?? string.Empty,
                    },
                }).ToList() ?? new List<CommentDto>()
            }).ToList();

            return postDtos;
        }

        // GET: api/PostApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPostModel(int id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (postModel == null)
            {
                return NotFound();
            }

            var postDto = new PostDto
            {
                PostId = postModel.PostId,
                Title = postModel.Title,
                Content = postModel.Content,
                CategoryId = postModel.CategoryId,
                DateCreated = postModel.DateCreated,
                DateModified = postModel.DateModified,
                User = new UserDto
                {
                    UserId = postModel.User?.Id ?? string.Empty,
                    UserName = postModel.User?.UserName ?? string.Empty,
                    FirstName = postModel.User?.FirstName ?? string.Empty,
                    LastName = postModel.User?.LastName ?? string.Empty,
                },
                Comments = postModel.Comments?.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    DateCreated = c.DateCreated,
                    DateModified = c.DateModified,
                    UserId = c.UserId,
                    PostId = c.PostId,
                    User = new UserDto
                    {
                        UserId = c.User?.Id ?? string.Empty,
                        UserName = c.User?.UserName ?? string.Empty,
                        FirstName = c.User?.FirstName ?? string.Empty,
                        LastName = c.User?.LastName ?? string.Empty,
                    },
                }).ToList() ?? new List<CommentDto>()
            };

            return postDto;
        }


        // PUT: api/PostApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostModel(int id, PostModel postModel)
        {
            if (id != postModel.PostId)
            {
                return BadRequest();
            }

            _context.Entry(postModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostModelExists(id))
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

        // POST: api/PostApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostModel>> PostPostModel(PostModel postModel)
        {
          if (_context.Posts == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
          }
            _context.Posts.Add(postModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostModel", new { id = postModel.PostId }, postModel);
        }

        // DELETE: api/PostApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostModel(int id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var postModel = await _context.Posts.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(postModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostModelExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }

        // GET: api/PostApi/MyPosts/userid
        [HttpGet("MyPosts/{uid}")]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetPostsByUserId(string uid)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }

            var userPosts = await _context.Posts
                .Where(post => post.UserId == uid)
                .ToListAsync();

            if (userPosts == null || userPosts.Count == 0)
            {
                return NotFound();
            }

            return userPosts;
        }

        // GET: api/PostApi/GetLatestPosts
        [HttpGet("GetLatestPosts")]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetLatestPosts()
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }

            var latestPosts = await _context.Posts
                .OrderByDescending(post => post.DateCreated) 
                .Take(3)
                .ToListAsync();

            if (latestPosts == null || latestPosts.Count == 0)
            {
                return NotFound();
            }

            return latestPosts;
        }


    }
}
