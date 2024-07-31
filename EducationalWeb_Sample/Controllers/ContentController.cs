using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EducationalWeb_Sample.Controllers
{
    
    [Route("Content")]
    public class ContentController : Controller
    {
        private readonly Courses courseApp;
        private readonly IMemoryCache memoryCache;

        public ContentController(Courses courseApp, IMemoryCache memoryCache)
        {
            this.courseApp = courseApp;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        private void MoveTempImgFile(string fileName)
        {
            string from = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/temp/img", fileName);
            string to = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/perm/img", fileName);

            if (System.IO.File.Exists(from))
            {
                var file = new FileInfo(from);
                file.MoveTo(to);
            }
        }

        private async Task CreateFileFromBuffer(string fileName)
        {
            if (!memoryCache.TryGetValue(fileName, out byte[] imgBytes))
            {
                throw new ArgumentException($"{fileName} was not found");
            }
            string to = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/perm/img", fileName);

            using (FileStream file = new FileStream(to, FileMode.Create, System.IO.FileAccess.Write))
            {
                await file.WriteAsync(imgBytes, 0, imgBytes.Length);
            }

            memoryCache.Remove(fileName);

        }

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

                CourseItem item = Utilities.CreateObjectBasedOn<CourseModel, CourseItem>(model);
                item.PostDate = DateTime.UtcNow;
                item.CourseLength = new Random().Next(1, 20);
                item.Students = new Random().Next(100, 500);
                item.Rating = (float)Math.Round((1 + new Random().NextDouble()) * 2.5, 1);

                await courseApp.InsertData(item);
                await CreateFileFromBuffer(item.ImageURL);

                return Redirect("/Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            //MoveTempImgFile(blogItem.ImageURL);
        }

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

        [HttpPost("UploadToTempFolder")]
        public async Task<IActionResult> UploadToTempFolder(IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    //size restriction
                    //image.Length

                    //Set Key Name
                    string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                    //Get url To Save
                    string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/temp/img", ImageName);

                    using (var stream = new FileStream(SavePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    return Ok(ImageName);
                }

                return BadRequest("Image not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }




        [HttpDelete("Delete/{id:int:required}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                CourseItem courseItem = (await courseApp.GetItemsByColumn("Id", id, outputColumn: ["Id", "ImageURL"]))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                await courseApp.DeleteData("Id", id);
                RemovePermImgFile(courseItem.ImageURL);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void RemovePermImgFile(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/perm/img", fileName);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

    }
}
