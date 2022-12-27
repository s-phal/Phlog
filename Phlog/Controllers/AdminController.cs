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

        public AdminController(ApplicationDbContext context, UserManager<SiteOwner> userManager, ImageService imageService) 
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
        }


        [Route("admin/")]
        public async Task<IActionResult> Dashboard()
        {
            var posts = await _context.Post
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            TempData["DisplayMessage"] = "Category created.";
            return Redirect("~/admin");
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
            return Redirect("~/admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Category category)
        {
            _context.Remove(category);
            await _context.SaveChangesAsync();

            TempData["DisplayMessage"] = "Category deleted.";
            return Redirect("~/admin");
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
                return Redirect("~/admin");
            }

            // check file integrity 
            if (!siteOwner.AvatarImageFile.ContentType.Contains("image"))
            {
                TempData["DisplayMessage"] = "Error - Picture file type not accepted.";
                return Redirect("~/admin");
            }

            user.AvatarFileName = _imageService.CreateUniqueFileName(siteOwner.AvatarImageFile);
            _imageService.UploadImageFile(siteOwner.AvatarImageFile, user.AvatarFileName);

            
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["DisplayMessage"] = "Profile Updated.";
            return Redirect("~/admin");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSiteSettings(SiteSettings siteSettings)
        {
            var getRow = await _context.SiteSettings
                .FirstOrDefaultAsync();

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
                return Redirect("~/admin");

            }


            return Redirect("~/admin");

        }

    }
}
