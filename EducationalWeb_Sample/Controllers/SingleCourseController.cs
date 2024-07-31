using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class SingleCourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
