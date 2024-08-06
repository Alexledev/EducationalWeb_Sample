using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class BlogController : Controller
    {
        private readonly Blogs courseApp;

        public BlogController(Blogs courseApp)
        {
            this.courseApp = courseApp;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<BlogItem> allBlogs = await courseApp.GetDataCollection();

            IEnumerable<BlogModel> blogModels = allBlogs.Select((e) => Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(e));

            return View(blogModels);
        }
    }
}
