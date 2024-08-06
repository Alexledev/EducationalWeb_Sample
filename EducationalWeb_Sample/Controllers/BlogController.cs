using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace EducationalWeb_Sample.Controllers
{
    [Route("Blog")]
    public class BlogController : Controller
    {

        private readonly int BlogsPerPage;
        private readonly Blogs blogApp;

        public BlogController(Blogs courseApp, IConfiguration configuration)
        {
            this.blogApp = courseApp;

            BlogsPerPage = (int)configuration.GetSection("ItemConfig").GetValue(typeof(int), "ItemsPerPage");
        }

        public async Task<IActionResult> Index()
        {
            return View(await GetData());
        }

        private async Task<IEnumerable<BlogModel>> GetData(int offset = 0)
        {
            (IEnumerable<BlogItem> allCourses, Int64 totalCourseCount) = await blogApp.GetCollectionDataWithCount(offset: offset, count: BlogsPerPage);

            IEnumerable<BlogModel> courseModels = allCourses.Select((e) => Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(e));

            ViewBag.pages = Convert.ToInt32(MathF.Ceiling(totalCourseCount / BlogsPerPage));
            ViewBag.totalCourseCount = totalCourseCount;

            return courseModels;
        }

        [HttpGet("page/{page:int}")]
        public async Task<IActionResult> Paging(int page)
        {
            int offset = (page - 1) * BlogsPerPage;

            return View("Index", await GetData(offset));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            IEnumerable<BlogItem> blogItems = await blogApp.FullTextSearch(keyword);
            IEnumerable<BlogModel> results = blogItems.Select((e) => Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(e));

            if (results.Count() == 0)
            {
                //ViewBag.couldntFindTerm = true;
                return RedirectToAction("Index");
            }

            return View("Index", results);
        }
    }
}
