using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize]
    public class BlogAdminController : Controller
    {
        private readonly Blogs blogApp;

        public BlogAdminController(Blogs courseApp)
        {
            this.blogApp = courseApp;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<BlogItem> allCourses = await blogApp.GetDataCollection();

            IEnumerable<BlogModel> courseModels = allCourses.Select((e) => Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(e));

            return View(courseModels);
        }
    }
}
