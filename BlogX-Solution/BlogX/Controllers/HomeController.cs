using BlogX.Migrations;
using BlogX.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using BlogX.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogX.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, UserManager<UserModel> _userManager)
        {
            this._userManager = _userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int categoryId, string orderByLatest, int page = 1, int pageSize = 10)
        {
            // Get authenticated user
            string userId = _userManager.GetUserId(this.User);

            // Get the count of posts for the current user
            int postCount = await _context.Posts.CountAsync(p => p.UserId == userId);

            // Get the count of comments for the current user
            int commentCount = await _context.Comments.CountAsync(c => c.UserId == userId);

            // Get the count of Users
            int userCount = await _context.Users.CountAsync();

            // Pass the counts to the view
            ViewData["UserID"] = userId;
            ViewData["PostCount"] = postCount;
            ViewData["CommentCount"] = commentCount;
            ViewData["UserCount"] = userCount;

            // Fetch posts with pagination
            var postsQuery = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsQueryable();

            // Apply search filter if search parameter is provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                postsQuery = postsQuery.Where(p =>
                    p.Title.ToLower().Contains(search) ||
                    p.Content.ToLower().Contains(search) ||
                    p.Category.CategoryName.ToLower().Contains(search)
                ) as IOrderedQueryable<PostModel>;
            }

            // Apply category filter if categoryId parameter is provided
            if (categoryId != 0)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId) as IOrderedQueryable<PostModel>;
            }

            // Convert the string value from the checkbox to a boolean
            bool orderByLatestBool = !string.IsNullOrEmpty(orderByLatest) && orderByLatest.ToLower() == "on";
            
            // Save OrderByLatest value to ViewData 
            ViewData["OrderByLatest"] = orderByLatestBool;

            // Order the posts based on the user's choice
            postsQuery = orderByLatestBool
                ? postsQuery.OrderByDescending(p => p.DateCreated)
                : postsQuery.OrderBy(p => p.DateCreated);


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


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}