using BlogApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class PageController : BaseController
    {
        public PageController(ICategoryRepository categoryRepository)
            : base(categoryRepository)
        {
        }

        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();
    }
}