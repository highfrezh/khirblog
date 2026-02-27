using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public AdminCategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: /Admin/AdminCategory/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var viewModel = categories.Select(c => new AdminCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                PostCount = c.BlogPosts.Count
            }).ToList();

            return View(viewModel);
        }

        // GET: /Admin/AdminCategory/Create
        public IActionResult Create()
        {
            return View(new AdminCategoryFormViewModel());
        }

        // POST: /Admin/AdminCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            await _categoryRepository.AddAsync(category);

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Admin/AdminCategory/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            var viewModel = new AdminCategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(viewModel);
        }

        // POST: /Admin/AdminCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = await _categoryRepository.GetByIdAsync(model.Id);
            if (category == null) return NotFound();

            category.Name = model.Name;
            category.Description = model.Description;

            await _categoryRepository.UpdateAsync(category);

            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/AdminCategory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            if (category.BlogPosts.Any())
            {
                TempData["Error"] = "Cannot delete category with existing posts.";
                return RedirectToAction("Index");
            }

            await _categoryRepository.DeleteAsync(id);

            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}