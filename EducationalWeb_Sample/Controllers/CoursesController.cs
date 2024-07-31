using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class CoursesController : Controller
    {
        private readonly Courses courseApp;

        public CoursesController(Courses courseApp)
        {
            this.courseApp = courseApp;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CourseItem> allCourses = await courseApp.GetDataCollection();

            IEnumerable<CourseModel> courseModels = allCourses.Select((e) => Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(e));

            return View(courseModels);
        }
    }
}
