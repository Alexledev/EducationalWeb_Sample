using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        private readonly int CoursesPerPage;
        private readonly Courses courseApp;

        public CoursesController(Courses courseApp, IConfiguration configuration)
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

            ViewBag.pages = Convert.ToInt32(MathF.Ceiling(totalCourseCount / CoursesPerPage));
            ViewBag.totalCourseCount = totalCourseCount;

            return courseModels;
        }

        [HttpGet("page/{page:int}")]
        public async Task<IActionResult> Paging(int page)
        {
            int offset = (page-1) * CoursesPerPage;
                        
            return View("Index", await GetData(offset));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {

            IEnumerable<CourseItem> blogItems = await courseApp.FullTextSearch(keyword);
            IEnumerable<CourseModel> results = blogItems.Select((e) => Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(e));

            if (results.Count() == 0)
            {
                //ViewBag.couldntFindTerm = true;
                return RedirectToAction("Index");
            }

            return View("Index", results);
        }
    }
}
