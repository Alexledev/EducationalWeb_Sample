using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
