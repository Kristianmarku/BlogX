using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BlogX.Models;
using Microsoft.EntityFrameworkCore;
using BlogX.Data;
using Microsoft.AspNetCore.Authorization;

namespace BlogX.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserModel> _userManager;

    public UserController(ApplicationDbContext context, UserManager<UserModel> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string search, string roleFilter, int page = 1, int pageSize = 10)
    {
        var usersWithRoles = await _userManager.Users
            .Include(u => u.Roles)
            .AsQueryable()
            .ToListAsync();

        // Apply search filter if search parameter is provided
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            usersWithRoles = usersWithRoles
                .Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(search)) ||
                    (u.Email != null && u.Email.ToLower().Contains(search))
                )
                .ToList();
        }

        // Apply role filter if roleFilter parameter is provided
        if (!string.IsNullOrEmpty(roleFilter))
        {
            usersWithRoles = usersWithRoles.Where(u =>
                _userManager.IsInRoleAsync(u, roleFilter).Result
            ).ToList();
        }

        // Paginate the results
        int totalUsers = usersWithRoles.Count;
        int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

        // Ensure the page is within a valid range
        page = Math.Max(1, Math.Min(page, totalPages));

        // Calculate the number of items to skip
        int skip = (page - 1) * pageSize;

        // Retrieve the users for the current page
        var users = usersWithRoles.Skip(skip).Take(pageSize).ToList();

        // Pass pagination information to the view
        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = totalPages;

        return View(users);
    }



    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserModel user, string[] selectedRoles)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;

            // Update user properties
            await _userManager.UpdateAsync(existingUser);

            // Get the current roles for the user
            var userRoles = await _userManager.GetRolesAsync(existingUser);

            // Add user to selected roles
            var rolesToAdd = selectedRoles.Except(userRoles);
            await _userManager.AddToRolesAsync(existingUser, rolesToAdd);

            // Remove user from roles not selected
            var rolesToRemove = userRoles.Except(selectedRoles);
            await _userManager.RemoveFromRolesAsync(existingUser, rolesToRemove);

            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }
}
