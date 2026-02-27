using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdminUserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUserController(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        // GET: /Admin/AdminUser/Index
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();

            var viewModel = new List<AdminUserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModel.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email!,
                    IsAdmin = user.IsAdmin,
                    Roles = roles.ToList()
                });
            }

            return View(viewModel);
        }

        // POST: /Admin/AdminUser/ToggleAdmin/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            // Prevent removing own admin role
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You cannot remove your own admin role.";
                return RedirectToAction("Index");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                user.IsAdmin = false;
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                user.IsAdmin = true;
            }

            await _userRepository.UpdateAsync(user);

            TempData["Success"] = "User role updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/AdminUser/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            // Prevent deleting own account
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Index");
            }

            await _userRepository.DeleteAsync(id);

            TempData["Success"] = "User deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}