using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EducationalWeb_Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Courses courseApp;

        public HomeController(ILogger<HomeController> logger, Courses courseApp)
        {
            _logger = logger;
            this.courseApp = courseApp;
        }
        public async Task<IActionResult> Index()
        {
            (IEnumerable<CourseItem> allCourses, long count) = await courseApp.GetCollectionDataWithCount(count: 5);
            ViewBag.CourseCount = count;
            IEnumerable<CourseModel> courseModels = allCourses.Select((e) => Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(e));

            return View(courseModels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
