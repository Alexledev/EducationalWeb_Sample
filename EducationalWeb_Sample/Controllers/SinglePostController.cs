using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace EducationalWeb_Sample.Controllers
{
    [Route("Blog")]
    public class SinglePostController : Controller
    {
        private readonly Blogs blogApp;

        public SinglePostController(Blogs courseApp)
        {
            this.blogApp = courseApp;
        }

        [HttpGet("{id:int:required}")]
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                BlogItem BI = (await blogApp.GetItemsByColumn("Id", id))?.SingleOrDefault() ?? throw new ArgumentException("Item not found");

                BlogModel blogModel = Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(BI);

                return View(blogModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
