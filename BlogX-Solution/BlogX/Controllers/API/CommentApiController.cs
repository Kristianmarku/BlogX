using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogX.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using BlogX.DTOs;

namespace BlogX.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CommentApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound();
            }

            var commentDtos = comments.Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                UserId = c.UserId,
                Content = c.Content,
                DateCreated = c.DateCreated,
                DateModified = c.DateModified,
                Post = new PostDto
                {
                    Title = c.Post?.Title ?? string.Empty,
                    Content = c.Post?.Content ?? string.Empty,
                    DateCreated = (DateTime)c.Post?.DateCreated,
                    DateModified = (DateTime)c.Post?.DateModified,

                },
                User = new UserDto
                {
                    UserName = c.User?.UserName ?? string.Empty,
                    FirstName = c.User?.FirstName ?? string.Empty,
                    LastName = c.User?.LastName ?? string.Empty,
                }
            }).ToList();

            return commentDtos;
        }

        // GET: api/CommentApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCommentModel(int id)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return NotFound();
            }

            var commentData = new
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                DateCreated = comment.DateCreated,
                DateModified = comment.DateModified,
            };

            return commentData;
        }

        // PUT: api/CommentApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommentModel(int id, CommentModel commentModel)
        {
            if (id != commentModel.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(commentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentModelExists(id))
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

        // POST: api/CommentApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CommentModel>> PostCommentModel(CommentModel commentModel)
        {
          if (_context.Comments == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Comments'  is null.");
          }
            _context.Comments.Add(commentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommentModel", new { id = commentModel.CommentId }, commentModel);
        }

        // DELETE: api/CommentApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentModel(int id)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
            var commentModel = await _context.Comments.FindAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("ByPostId/{postId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPostId(int postId)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.Post)
                .Include(c => c.User)
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound();
            }

            var commentDtos = comments.Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                UserId = c.UserId,
                Content = c.Content,
                DateCreated = c.DateCreated,
                DateModified = c.DateModified,
                Post = new PostDto
                {
                    Title = c.Post?.Title ?? string.Empty,
                    Content = c.Post?.Content ?? string.Empty,
                    DateCreated = (DateTime)c.Post?.DateCreated,
                    DateModified = (DateTime)c.Post?.DateModified,
                },
                User = new UserDto
                {
                    UserName = c.User?.UserName ?? string.Empty,
                    FirstName = c.User?.FirstName ?? string.Empty,
                    LastName = c.User?.LastName ?? string.Empty,
                }
            }).ToList();

            return commentDtos;
        }

        [HttpGet("ByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByUserId(string userId)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.Post)
                .Include(c => c.User)
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound();
            }

            var commentDtos = comments.Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                UserId = c.UserId,
                Content = c.Content,
                DateCreated = c.DateCreated,
                DateModified = c.DateModified,
                Post = new PostDto
                {
                    Title = c.Post?.Title ?? string.Empty,
                    Content = c.Post?.Content ?? string.Empty,
                    DateCreated = (DateTime)c.Post?.DateCreated,
                    DateModified = (DateTime)c.Post?.DateModified,
                },
                User = new UserDto
                {
                    UserName = c.User?.UserName ?? string.Empty,
                    FirstName = c.User?.FirstName ?? string.Empty,
                    LastName = c.User?.LastName ?? string.Empty,
                }
            }).ToList();

            return commentDtos;
        }

        private bool CommentModelExists(int id)
        {
            return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
