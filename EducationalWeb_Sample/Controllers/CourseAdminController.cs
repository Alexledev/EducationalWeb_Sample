using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize(Roles = "CourseAdmin")]
    [Route("CourseAdmin")]
    public class CourseAdminController : Controller
    {
        private readonly int CoursesPerPage;
        private readonly Courses courseApp;
        private readonly IMemoryCache memoryCache;

        public CourseAdminController(Courses courseApp, IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.courseApp = courseApp;
            this.memoryCache = memoryCache;
            CoursesPerPage = (int)configuration.GetSection("ItemConfig").GetValue(typeof(int), "ItemsPerPage");
            
        }

        [Authorize(Roles = "CourseAdmin", Policy = "CanReadCourse")]
        public async Task<IActionResult> Index()
        {            
            return View(await GetData());
        }

        [Authorize(Policy = "CanCreateCourse")]
        [HttpGet("AddItem")]
        public IActionResult AddItem()
        {
            return View();
        }

        private async Task<IEnumerable<CourseModel>> GetData(int offset = 0)
        {
            (IEnumerable<CourseItem> allCourses, Int64 totalCourseCount) = await courseApp.GetCollectionDataWithCount(offset: offset, count: CoursesPerPage);

            IEnumerable<CourseModel> courseModels = allCourses.Select((e) => Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(e));

            var ceil = MathF.Ceiling((float)totalCourseCount / (float)CoursesPerPage);
            ViewBag.pages = Convert.ToInt32(ceil);
            ViewBag.totalCourseCount = totalCourseCount;

            return courseModels;
        }


        [Authorize(Policy = "CanReadCourse")]
        [HttpGet("page/{page:int}")]
        public async Task<IActionResult> Paging(int page)
        {
            int offset = (page - 1) * CoursesPerPage;
            ViewBag.currentPage = page;

            return View("Index", await GetData(offset));
        }


        [Authorize(Roles = "CourseAdmin", Policy = "CanEditCourse")]
        [HttpGet("Edit/{id:int:required}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                CourseItem blogItem = (await courseApp.GetItemsByColumn("Id", id))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                CourseModel model = Utilities.CreateObjectBasedOn<CourseItem, CourseModel>(blogItem);

                ViewBag.Id = id;

                return View(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "CanReadCourse")]
        [HttpPost("Update/{id:int:required}")]
        public async Task<IActionResult> UpdateContent(CourseModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsInvalid = true;
                ViewBag.Id = id;
                return View("Edit", model);
            }

            try
            {
                CourseItem blogItem = (await courseApp.GetItemsByColumn("Id", id))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                blogItem.Summary = model.Summary.Trim();
                blogItem.Description = model.Description.Trim();
                blogItem.Title = model.Title.Trim();
                blogItem.ImageURL = model.ImageURL.Trim();
                blogItem.Topic = model.Topic.Trim();
                blogItem.Price = model.Price;


                await courseApp.UpdateData(blogItem, "Id");
                await Utilities.CreateFileFromBuffer(blogItem.ImageURL, memoryCache, true);

                // MoveTempImgFile(blogItem.ImageURL);


                return Redirect("/CourseAdmin");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Authorize(Policy = "CanCreateCourse")]
        [HttpPost("AddContent")]
        public async Task<IActionResult> AddContent(CourseModel model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.IsInvalid = true;
                return View("Index", model);
            }

            try
            {
                model.Title = model.Title.Trim();
                model.Summary = model.Summary.Trim();
                model.Description = model.Description.Trim();
                model.Topic = model.Topic.Trim();

                CourseItem item = Utilities.CreateObjectBasedOn<CourseModel, CourseItem>(model);
                item.PostDate = DateTime.UtcNow;
                item.CourseLength = new Random().Next(1, 20);
                item.Students = new Random().Next(100, 500);
                item.Rating = (float)Math.Round((1 + new Random().NextDouble()) * 2.5, 1);

                await courseApp.InsertData(item);
                await Utilities.CreateFileFromBuffer(item.ImageURL, memoryCache);

                return Redirect("/CourseAdmin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            //MoveTempImgFile(blogItem.ImageURL);
        }

        [Authorize(Policy = "CanEditOrCreateCourse")]
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    if (image.Length >= 1048576)
                    {
                        throw new ArgumentException("File size must be under 1 MB");
                    }

                    // await Task.Delay(5000);

                    //Set Key Name
                    string ImageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                    byte[] buffer = new byte[image.Length];

                    using (var stream = new MemoryStream(buffer))
                    {
                        await image.CopyToAsync(stream);
                    }
                    memoryCache.Set(ImageName, buffer, DateTimeOffset.UtcNow.AddMinutes(60));

                    return Ok(ImageName);
                }

                return BadRequest("Image not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [Authorize(Policy = "CanDeleteCourse")]
        [HttpDelete("Delete/{id:int:required}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                CourseItem courseItem = (await courseApp.GetItemsByColumn("Id", id, outputColumn: ["Id", "ImageURL"]))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                await courseApp.DeleteData("Id", id);
                Utilities.RemovePermImgFile(courseItem.ImageURL);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
