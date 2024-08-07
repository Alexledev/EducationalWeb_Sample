using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize]
    [Route("CourseAdmin")]
    public class CourseAdminController : Controller
    {
        private readonly int CoursesPerPage;
        private readonly Courses courseApp;

        public CourseAdminController(Courses courseApp, IConfiguration configuration)
        {
            this.courseApp = courseApp;

            CoursesPerPage = (int)configuration.GetSection("ItemConfig").GetValue(typeof(int), "ItemsPerPage");
        }

        public async Task<IActionResult> Index()
        {            
            return View(await GetData());
        }

        private async Task<IEnumerable<CourseModel>> GetData(int offset = 0)
        {
            (IEnumerable<CourseItem> allCourses, Int64 totalCourseCount) = await courseApp.GetCollectionDataWithCount(offset: offset, count: CoursesPerPage);

            IEnumerable<CourseModel> courseModels = allCourses.Select((e) => Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(e));

            var ceil = MathF.Ceiling((float)totalCourseCount / (float)CoursesPerPage);
            ViewBag.pages = Convert.ToInt32(ceil);
            ViewBag.totalCourseCount = totalCourseCount;

            return courseModels;
        }

        [HttpGet("page/{page:int}")]
        public async Task<IActionResult> Paging(int page)
        {
            int offset = (page - 1) * CoursesPerPage;
            ViewBag.currentPage = page;

            return View("Index", await GetData(offset));
        }
    }
}
