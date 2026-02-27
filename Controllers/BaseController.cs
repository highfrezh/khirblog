using BlogApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public BaseController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.HeaderCategories = categories;
            await next();
        }
    }
}