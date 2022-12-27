using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Phlog.Data;
using Phlog.Models;
using Phlog.Services;

namespace Phlog.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<SiteOwner> _userManager;
        private readonly ImageService _imageService;
        private readonly TagService _tagService;

        public AdminController(ApplicationDbContext context, UserManager<SiteOwner> userManager, ImageService imageService, TagService tagService) 
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _tagService = tagService;
        }


        [Route("admin/")]
        [Route("admin/post")]
        public async Task<IActionResult> Dashboard()
        {
            var posts = await _context.Post
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View("post", posts);
        }

        public async Task<IActionResult> CreatePost(Post post, Tag tag)
        {
            post.ImageFileName = _imageService.CreateUniqueFileName(post.ImageFile);
            _imageService.UploadImageFile(post.ImageFile, post.ImageFileName);
            _context.Add(post);
            await _context.SaveChangesAsync();

            var tagValues = _tagService.SplitTags(tag.Name);

            foreach (var aTag in tagValues)
            {
                Tag newTag = new Tag()
                {
                    Name = aTag,
                    PostId = post.Id
                };
                _context.Add(newTag);
            }

            await _context.SaveChangesAsync();

            TempData["DisplayMessage"] = "Post created.";
            return Redirect("~/admin");
        }

        [Route("admin/category")]
        public async Task<IActionResult> Category()
        {
            var category = await _context.Category.ToListAsync();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if(category.Name == null)
            {
                TempData["DisplayMessage"] = "Error - Category name can not be empty.";
                return Redirect("~/admin/category");
            }

            _context.Add(category);
            await _context.SaveChangesAsync();
            TempData["DisplayMessage"] = "Category created.";
            return Redirect("~/admin/category");
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            var getCategory = await _context.Category
                .Where(c => c.Id == category.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            _context.Update(category);
            await _context.SaveChangesAsync();

            TempData["DisplayMessage"] = "Category Updated.";
            return Redirect("~/admin/category");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Category category)
        {
            _context.Remove(category);
            await _context.SaveChangesAsync();

            TempData["DisplayMessage"] = "Category deleted.";
            return Redirect("~/admin/category");
        }

        [Route("~/admin/ownerprofile")]
        public async Task<IActionResult> OwnerProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            return View("ownerprofile", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOwnerProfile(SiteOwner siteOwner)
        {
            var user = await _userManager.GetUserAsync(User);

            user.FirstName = siteOwner.FirstName;
            user.LastName = siteOwner.LastName;
            user.AboutMe = siteOwner.AboutMe;
            user.PhoneNumber = siteOwner.PhoneNumber;
            user.InstagramId = siteOwner.InstagramId;
            user.FaceBookId = siteOwner.FaceBookId;
            user.TwitterId = siteOwner.TwitterId;

            if (siteOwner.AvatarImageFile == null)
            {
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();

                TempData["DisplayMessage"] = "Profile Updated.";
                return Redirect("~/admin/ownerprofile");
            }

            // check file integrity 
            if (!siteOwner.AvatarImageFile.ContentType.Contains("image"))
            {
                TempData["DisplayMessage"] = "Error - Picture file type not accepted.";
                return Redirect("~/admin/ownerprofile");
            }

            user.AvatarFileName = _imageService.CreateUniqueFileName(siteOwner.AvatarImageFile);
            _imageService.UploadImageFile(siteOwner.AvatarImageFile, user.AvatarFileName);

            
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["DisplayMessage"] = "Profile Updated.";
            return Redirect("~/admin/ownerprofile");
        }

        [Route("~/admin/settings")]
        public async Task<IActionResult> SiteSettings()
        {
            var siteSettings = await _context.SiteSettings.FirstOrDefaultAsync();
            return View("settings", siteSettings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSiteSettings(SiteSettings siteSettings)
        {
            var getRow = await _context.SiteSettings
                .FirstOrDefaultAsync();

            if(siteSettings.SiteName == null)
            {
                TempData["DisplayMessage"] = "Error - Site name can not be empty.";
                return Redirect("~/admin/settings");

            }

            if (getRow == null) 
            {

                _context.Add(siteSettings);
                await _context.SaveChangesAsync();

                TempData["DisplayMessage"] = "Settings Updated.";
                return Redirect("~/admin");
            }

            if (getRow != null )
            {
                getRow.SiteDescription = siteSettings.SiteDescription;
                getRow.SiteName= siteSettings.SiteName;

                _context.Update(getRow);
                await _context.SaveChangesAsync();

                TempData["DisplayMessage"] = "Settings Updated.";
                return Redirect("~/admin/settings");

            }


            return Redirect("~/admin");

        }

    }
}
