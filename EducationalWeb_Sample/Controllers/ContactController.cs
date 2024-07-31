using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
