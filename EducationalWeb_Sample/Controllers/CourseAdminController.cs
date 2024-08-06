using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize]
    public class CourseAdminController : Controller
    {
        private readonly Courses courseApp;

        public CourseAdminController(Courses courseApp)
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
