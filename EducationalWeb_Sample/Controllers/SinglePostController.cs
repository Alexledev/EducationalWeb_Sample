using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class SinglePostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
