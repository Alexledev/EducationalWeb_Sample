using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    [Route("Course")]
    public class SingleCourseController : Controller
    {
        private readonly Courses courseApp;
        public SingleCourseController(Courses courseApp)
        {
            this.courseApp = courseApp;
        }

        [HttpGet("{id:int:required}")]
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                CourseItem CI = (await courseApp.GetItemsByColumn("Id", id))?.SingleOrDefault() ?? throw new ArgumentException("Item not found");

                CourseModel courseModel = Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(CI);

                return View(courseModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
