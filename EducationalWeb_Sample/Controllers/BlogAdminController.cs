using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize]
    [Route("BlogAdmin")]
    public class BlogAdminController : Controller
    {
        private readonly int BlogsPerPage;
        private readonly Blogs blogApp;

        public BlogAdminController(Blogs courseApp, IConfiguration configuration)
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
            (IEnumerable<BlogItem> allBlogs, Int64 totalBlogCount) = await blogApp.GetCollectionDataWithCount(offset: offset, count: BlogsPerPage);

            IEnumerable<BlogModel> blogModels = allBlogs.Select((e) => Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(e));

            var ceil = MathF.Ceiling((float)totalBlogCount / (float)BlogsPerPage);
            ViewBag.pages = Convert.ToInt32(ceil);
            ViewBag.totalBlogCount = totalBlogCount;

            return blogModels;
        }

        [HttpGet("page/{page:int}")]
        public async Task<IActionResult> Paging(int page)
        {
            int offset = (page - 1) * BlogsPerPage;
            ViewBag.currentPage = page;

            return View("Index", await GetData(offset));
        }
    }
}
