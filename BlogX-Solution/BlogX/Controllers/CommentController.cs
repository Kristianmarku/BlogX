using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogX.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogX.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<UserModel> _userManager)
        {
            _context = context;
            this._userManager = _userManager;
        }

        // GET: Comment
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            // Check if user is admin
            bool isAdmin = User.IsInRole("Admin");

            IQueryable<CommentModel> commentsQuery;

            if (isAdmin)
            {
                // If the user is an admin, show all comments
                commentsQuery = _context.Comments.Include(c => c.Post)
                                                  .Include(c => c.User);
            }
            else
            {
                // If the user is not an admin, show only their own comments
                string userId = _userManager.GetUserId(User);
                commentsQuery = _context.Comments.Include(c => c.Post)
                                            .Include(c => c.User)
                                            .Where(c => c.UserId == userId);
            }

            // Apply search filter if search parameter is provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                commentsQuery = commentsQuery.Where(c =>
                    c.Content.ToLower().Contains(search) ||
                    c.Post.Title.ToLower().Contains(search)
                );
            }

            // Paginate the results
            int totalComments = await commentsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalComments / pageSize);

            // Ensure the page is within a valid range
            page = Math.Max(1, Math.Min(page, totalPages));

            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize;

            // Retrieve the comments for the current page
            var comments = await commentsQuery.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination information to the view
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(comments);
        }


        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var commentModel = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (commentModel == null)
            {
                return NotFound();
            }

            return View(commentModel);
        }

        // GET: Comment/Create
        public IActionResult Create(int postId)
        {
            ViewData["PostId"] = postId;
            return View();
        }

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postId, [Bind("CommentId,PostId,UserId,Content,DateCreated,DateModified")] CommentModel commentModel)
        {
            if (ModelState.IsValid)
            {
                // Set the current date for DateCreated and DateModified
                commentModel.DateCreated = DateTime.Now;
                commentModel.DateModified = DateTime.Now;

                commentModel.PostId = postId;
                commentModel.UserId = _userManager.GetUserId(this.User);

                _context.Add(commentModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Post", new { id = commentModel.PostId });
                //return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "PostId", "Content", commentModel.PostId);
            return View(commentModel);
        }

        // GET: Comment/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var commentModel = await _context.Comments.FindAsync(id);

            // Check if the user is authorized to edit the post
            if (!IsAuthorizedToEdit(commentModel))
            {
                return Forbid(); // Or redirect to an unauthorized page
            }

            if (commentModel == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "PostId", "Content", commentModel.PostId);
            return View(commentModel);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,PostId,UserId,Content,DateCreated,DateModified")] CommentModel commentModel)
        {
            if (id != commentModel.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing comment from the database
                    var existingComment = await _context.Comments.FindAsync(id);

                    // Update only the necessary fields
                    existingComment.Content = commentModel.Content;
                    existingComment.DateModified = DateTime.Now;

                    _context.Update(existingComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentModelExists(commentModel.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "PostId", "Content", commentModel.PostId);
            return View(commentModel);
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var commentModel = await _context.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (commentModel == null)
            {
                return NotFound();
            }

            return View(commentModel);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Comments'  is null.");
            }
            var commentModel = await _context.Comments.FindAsync(id);
            if (commentModel != null)
            {
                _context.Comments.Remove(commentModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if the user is authorized to edit the post
        private bool IsAuthorizedToEdit(CommentModel commentModel)
        {
            if (commentModel == null)
            {
                return false;
            }

            string currentUserId = _userManager.GetUserId(User);

            // Check if the user is the owner of the post or is in the "Admin" role
            return User.IsInRole("Admin") || commentModel.UserId == currentUserId;
        }

        private bool CommentModelExists(int id)
        {
          return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
