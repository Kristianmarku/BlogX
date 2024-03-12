using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogX.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace BlogX.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(ApplicationDbContext context, UserManager<UserModel> _userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._userManager = _userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Post
        public async Task<IActionResult> Index(string search, int categoryId, int page = 1, int pageSize = 10)
        {
            // Check if user is admin
            bool isAdmin = User.IsInRole("Admin");

            IQueryable<PostModel> postsQuery;

            if (isAdmin)
            {
                // If the user is an admin, show all posts
                postsQuery = _context.Posts.Include(p => p.Category);
            }
            else
            {
                // If the user is not an admin, show only their own posts
                string userId = _userManager.GetUserId(User);
                postsQuery = _context.Posts.Include(p => p.Category)
                                           .Where(p => p.UserId == userId);
            }

            // Apply search filter if search parameter is provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                postsQuery = postsQuery.Where(p =>
                    p.Title.ToLower().Contains(search) ||
                    p.Content.ToLower().Contains(search) ||
                    p.Category.CategoryName.ToLower().Contains(search)
                );
            }

            // Apply category filter if categoryId parameter is provided
            if (categoryId != 0)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId);
            }

            // Paginate the results
            int totalPosts = await postsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            // Ensure the page is within a valid range
            page = Math.Max(1, Math.Min(page, totalPages));

            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize;

            // Retrieve the posts for the current page
            var posts = await postsQuery.Skip(skip).Take(pageSize).ToListAsync();

            // Pass pagination information to the view
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            // Pass category information to the view
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);

            return View(posts);
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.Comments) // Include comments for eager loading
                    .ThenInclude(p => p.User)
                .Include(c => c.User)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // GET: Post/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View(new PostModel());
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Content,CategoryId,UserId,DateCreated,DateModified,ImageFile")] PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                postModel.DateCreated = DateTime.Now;
                postModel.DateModified = DateTime.Now;
                postModel.UserId = _userManager.GetUserId(this.User);

                if (postModel.ImageFile != null && postModel.ImageFile.Length > 0)
                {
                    // Save the uploaded image to a folder on the server
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + postModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    postModel.ImageUrl = "/images/" + uniqueFileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postModel.ImageFile.CopyToAsync(stream);
                    }
                }

                _context.Add(postModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
             
            }
            return View(postModel);
        }

        // GET: Post/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts.FindAsync(id);

            // Check if the user is authorized to edit the post
            if (!IsAuthorizedToEdit(postModel))
            {
                return Forbid(); // Or redirect to an unauthorized page
            }

            if (postModel == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", postModel.CategoryId);
            return View(postModel);
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,CategoryId,UserId,DateCreated,DateModified,ImageFile")] PostModel postModel)
        {
            if (id != postModel.PostId)
            {
                return NotFound();
            }

            // Check if the user is authorized to edit the post
            if (!IsAuthorizedToEdit(postModel))
            {
                return Forbid(); // Or redirect to an unauthorized page
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing post from the database
                    var existingPost = await _context.Posts.FindAsync(id);

                    // Update properties that can be modified
                    existingPost.Title = postModel.Title;
                    existingPost.Content = postModel.Content;
                    existingPost.CategoryId = postModel.CategoryId;
                    existingPost.DateModified = DateTime.Now;

                    // Check if a new image is uploaded
                    if (postModel.ImageFile != null && postModel.ImageFile.Length > 0)
                    {
                        // Save the uploaded image to a folder on the server
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                        // Create the directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + postModel.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        existingPost.ImageUrl = "/images/" + uniqueFileName;

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await postModel.ImageFile.CopyToAsync(stream);
                        }
                    }

                    // Update the database
                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostModelExists(postModel.PostId))
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

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", postModel.CategoryId);
            return View(postModel);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var postModel = await _context.Posts.FindAsync(id);
            if (postModel != null)
            {
                _context.Posts.Remove(postModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if the user is authorized to edit the post
        private bool IsAuthorizedToEdit(PostModel postModel)
        {
            if (postModel == null)
            {
                return false;
            }

            string currentUserId = _userManager.GetUserId(User);

            // Check if the user is the owner of the post or is in the "Admin" role
            return User.IsInRole("Admin") || postModel.UserId == currentUserId;
        }

        private bool PostModelExists(int id)
        {
          return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
