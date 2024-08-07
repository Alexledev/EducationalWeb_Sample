using Application;
using BlogSample.Models;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EducationalWeb_Sample.Controllers
{
    [Authorize(Roles = "BlogAdmin")]
    [Route("BlogAdmin")]
    public class BlogAdminController : Controller
    {
        private readonly int BlogsPerPage;
        private readonly Blogs blogApp;
        private readonly IMemoryCache memoryCache;

        public BlogAdminController(Blogs courseApp, IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.blogApp = courseApp;
            this.memoryCache = memoryCache;

            BlogsPerPage = (int)configuration.GetSection("ItemConfig").GetValue(typeof(int), "ItemsPerPage");
        }

        [Authorize(Policy = "CanReadBlog")]
        public async Task<IActionResult> Index()
        {           
            return View(await GetData());
        }

        private async Task<IEnumerable<BlogModel>> GetData(int offset = 0)
        {
            (IEnumerable<BlogItem> allBlogs, Int64 totalBlogCount) = await blogApp.GetCollectionDataWithCount(offset: offset, count: BlogsPerPage);

            IEnumerable<BlogModel> blogModels = allBlogs.Select((e) => Utilities.CreateObjectBasedOn<BlogItem, BlogModel>(e));

            var ceil = MathF.Ceiling((float)totalBlogCount / (float)BlogsPerPage);
            ViewBag.pages = Convert.ToInt32(ceil);
            ViewBag.totalBlogCount = totalBlogCount;

            return blogModels;
        }

        [Authorize(Policy = "CanReadBlog")]
        [HttpGet("page/{page:int}")]
        public async Task<IActionResult> Paging(int page)
        {
            int offset = (page - 1) * BlogsPerPage;
            ViewBag.currentPage = page;

            return View("Index", await GetData(offset));
        }

        [Authorize(Policy = "CanCreateBlog")]
        [HttpGet("AddItem")]
        public IActionResult AddItem()
        {
            return View();
        }

        [Authorize(Policy = "CanCreateBlog")]
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
                await Utilities.CreateFileFromBuffer(item.ImageURL, memoryCache);

                return Redirect("/BlogAdmin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "CanEditBlog")]
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

        [Authorize(Policy = "CanReadBlog")]
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
                await Utilities.CreateFileFromBuffer(blogItem.ImageURL, memoryCache, true);

                // MoveTempImgFile(blogItem.ImageURL);


                return Redirect("/BlogAdmin");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Authorize(Policy = "CanEditOrCreateBlog")]
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

        [Authorize(Policy = "CanDeleteBlog")]
        [HttpDelete("Delete/{id:int:required}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                BlogItem courseItem = (await blogApp.GetItemsByColumn("Id", id, outputColumn: ["Id", "ImageURL"]))?.SingleOrDefault() ??
                                                                             throw new ArgumentException("Item doesn't exist");

                await blogApp.DeleteData("Id", id);
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
