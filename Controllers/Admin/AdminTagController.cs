using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdminTagController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public AdminTagController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        // GET: /Admin/AdminTag/Index
        public async Task<IActionResult> Index()
        {
            var tags = await _tagRepository.GetAllAsync();

            var viewModel = tags.Select(t => new AdminTagViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                PostCount = t.BlogPostTags.Count
            }).ToList();

            return View(viewModel);
        }

        // GET: /Admin/AdminTag/Create
        public IActionResult Create()
        {
            return View(new AdminTagFormViewModel());
        }

        // POST: /Admin/AdminTag/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminTagFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tag = new Tag
            {
                Name = model.Name,
                Description = model.Description
            };

            await _tagRepository.AddAsync(tag);

            TempData["Success"] = "Tag created successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Admin/AdminTag/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return NotFound();

            var viewModel = new AdminTagFormViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description
            };

            return View(viewModel);
        }

        // POST: /Admin/AdminTag/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminTagFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tag = await _tagRepository.GetByIdAsync(model.Id);
            if (tag == null) return NotFound();

            tag.Name = model.Name;
            tag.Description = model.Description;

            await _tagRepository.UpdateAsync(tag);

            TempData["Success"] = "Tag updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/AdminTag/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return NotFound();

            await _tagRepository.DeleteAsync(id);

            TempData["Success"] = "Tag deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}