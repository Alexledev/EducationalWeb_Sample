using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
