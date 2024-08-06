using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize]
    [Route("BlogContent")]
    public class BlogContentController : Controller
    {
        private readonly Blogs blogApp;
        private readonly IMemoryCache memoryCache;

        public BlogContentController(Blogs blogApp, IMemoryCache memoryCache)
        {
            this.blogApp = blogApp;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Edit/{id:int:required}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                BlogItem blogItem = (await blogApp.GetItemsByColumn("Id", id))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                BlogModel model = Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(blogItem);

                ViewBag.Id = id;

                return View(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddContent")]
        public async Task<IActionResult> AddContent(BlogModel model)
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
                model.Category = model.Category.Trim();

                BlogItem item = Utilities.CreateObjectBasedOn<BlogModel, BlogItem>(model);
                item.PostDate = DateTime.UtcNow;
                item.Author = this.HttpContext.User.Identity!.Name!;

                await blogApp.InsertData(item);
                await CreateFileFromBuffer(item.ImageURL);

                return Redirect("/BlogAdmin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Update/{id:int:required}")]
        public async Task<IActionResult> UpdateContent(BlogModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsInvalid = true;
                ViewBag.Id = id;
                return View("Edit", model);
            }

            try
            {
                BlogItem blogItem = (await blogApp.GetItemsByColumn("Id", id))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                blogItem.Summary = model.Summary.Trim();
                blogItem.Description = model.Description.Trim();
                blogItem.Title = model.Title.Trim();
                blogItem.ImageURL = model.ImageURL.Trim();
                blogItem.Category = model.Category.Trim();


                await blogApp.UpdateData(blogItem, "Id");
                await CreateFileFromBuffer(blogItem.ImageURL, true);

                // MoveTempImgFile(blogItem.ImageURL);


                return Redirect("/BlogAdmin");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        private async Task CreateFileFromBuffer(string fileName, bool editing = false)
        {
            bool cantGetMemCacheValue = !memoryCache.TryGetValue(fileName, out byte[] imgBytes);

            if (editing == true && cantGetMemCacheValue)
            {
                return;
            }

            if (cantGetMemCacheValue)
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

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile image)
        {
            try
            {
                if (image == null)
                {
                    return BadRequest("Image not found");
                }

                if (image.Length >= 1048576)
                {
                    throw new ArgumentException("File size must be under 1 MB");
                }

                string ImageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                byte[] buffer = new byte[image.Length];

                using (var stream = new MemoryStream(buffer))
                {
                    await image.CopyToAsync(stream);
                }
                memoryCache.Set(ImageName, buffer, DateTimeOffset.UtcNow.AddMinutes(60));

                return Ok(ImageName);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id:int:required}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                BlogItem courseItem = (await blogApp.GetItemsByColumn("Id", id, outputColumn: ["Id", "ImageURL"]))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                await blogApp.DeleteData("Id", id);
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
