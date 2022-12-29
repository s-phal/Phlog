using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Phlog.Data;
using Phlog.Models;
using Phlog.Services;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

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

        [HttpPost]
        public async Task<IActionResult> CreatePost(Post post, Tag tag)
        {
            // assign value 'unkown' if model name was not provided
            if (post.ModelName == null) post.ModelName = "unknown";

            // assign value 'unknown' if instagram name was not provided
            if (post.InstagramUsername == null) post.InstagramUsername = "unknown";

            // reject file if not valid
            if (!post.ImageFile.ContentType.Contains("image"))
            {
                TempData["DisplayMessage"] = "Error - File type not accepted.";
                return Redirect("~/admin/post");
            }

            // assign a unique file name for the property
            post.ImageFileName = _imageService.CreateUniqueFileName(post.ImageFile);

            // upload the attached imagefile to the server
            _imageService.UploadImageFile(post.ImageFile, post.ImageFileName);

            

            // track and save changes to the database
            _context.Add(post);
            await _context.SaveChangesAsync();
            
            // add tag values
            if (tag.Name != null)
            {
                // split the string of tag values into singles 
                // and assign them to an array
                var tagValues = _tagService.SplitTags(tag.Name);

                // create a new instance for each tag
                // track each instance
                // save changes to the database
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
            }


            TempData["DisplayMessage"] = "Post created.";
            return Redirect("~/admin");
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(Post post, Tag tag)
        {
            if (tag.Name != null)
            {
                var getDbTagValue = _context.Tag.Where(t => t.PostId == post.Id).AsEnumerable();

                _context.RemoveRange(getDbTagValue);
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
            }

            if (tag.Name == null) 
            {
                var getDbTagValue = _context.Tag.Where(t => t.PostId == post.Id).AsEnumerable();

                _context.RemoveRange(getDbTagValue);
                await _context.SaveChangesAsync();

            }

            if (post.ImageFile == null)
            {
                var getDbValue = await _context.Post
                    .AsNoTracking()
                    .Where(p => p.Id == post.Id)
                    .FirstOrDefaultAsync();
                post.ImageFileName = getDbValue.ImageFileName;

                _context.Update(post);
                await _context.SaveChangesAsync();
                return Redirect("~/admin/post");
            }

            post.ImageFileName = _imageService.CreateUniqueFileName(post.ImageFile);
            _imageService.UploadImageFile(post.ImageFile, post.ImageFileName);

            _context.Update(post);

            await _context.SaveChangesAsync();


            TempData["DisplayMessage"] = "Post Updated.";
            return Redirect("~/admin/post");

        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePost(int id)
		{
			if (_context.Post == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Post'  is null.");
			}
			var post = await _context.Post.FindAsync(id);
			if (post != null)
			{
				_context.Post.Remove(post);
			}

			await _context.SaveChangesAsync();

			TempData["DisplayMessage"] = "Post deleted.";
			return Redirect("~/admin");
		}

		[Route("~/admin/search/{wildcard?}")]
        public async Task<IActionResult> Search(string? s, string? m, string? c, string? i, string? t)
        {
            // assign variables for easier reading
            string searchTerm = s;
            string modelName = m;
            string categoryName = c;
            string instagramName = i;
            string tagName = t;

            // if search input was provided
            if (searchTerm != null)
            {
                // must exceed 3 characters to perform search
                if (searchTerm.Length < 3)
                {
                    TempData["DisplayMessage"] = "Error - Search term must be at least 3 characters long.";
                    return Redirect("~/admin/post");
                }

                // convert string for case sensitivity
                searchTerm = searchTerm.ToLower();

                // fetch all post that contains the searchTerm
                // in the corresponding columns
                var posts = await _context.Post
                    .Where(p => p.Id.ToString().Contains(searchTerm) || 
                    p.ModelName.ToLower().Contains(searchTerm) ||
                    p.Category.Name.ToLower().Contains(searchTerm) ||
                    p.InstagramUsername.ToLower().Contains(searchTerm) ||
                    p.Tags.Any(t => t.Name.ToLower().Contains(searchTerm)))
                    .ToListAsync();

                return View("post", posts);
            }

            // filter by model name
            if (modelName != null)
            {
                // convert string for case sensitivity
                modelName = modelName.ToLower();

                // fetch all post that contains model name
                var posts = await _context.Post
                    .Where(p => p.ModelName.ToLower() == modelName)
                    .ToListAsync();

                return View("post", posts);
            }

            // filter by category name
            if (categoryName != null)
            {
                // convert string for case sensitivity
                categoryName = categoryName.ToLower();

                // fetch all post that contains category name
                var posts = await _context.Post
                    .Where(p => p.Category.Name.ToLower() == categoryName)
                    .ToListAsync();

                return View("post", posts);
            }

            // filter by instagram name
            if (instagramName != null)
            {
                // convert string for case sensitivity
                instagramName = instagramName.ToLower();

                // fetch all post that contains the instagram name
                var posts = await _context.Post
                    .Where(p => p.InstagramUsername.ToLower() == instagramName)
                    .ToListAsync();

                return View("post", posts);
            }

            // filter by tag value
            if (tagName != null)
            {
                // convert string for case sensitivity
                tagName = tagName.ToLower();

                // fetch all post that contains the tag value
                var posts = await _context.Post
                    .Where(p => p.Tags.Any(t => t.Name.ToLower().Contains(tagName)))
                    .ToListAsync();

                return View("post", posts);
            }

            // redirect if input is empty string
            if (searchTerm == null) return Redirect("~/admin/post");

            return Redirect("~/admin/");


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

            // reject file if not valid
            if (!siteOwner.AvatarImageFile.ContentType.Contains("image"))
            {
                TempData["DisplayMessage"] = "Error - File type not accepted.";
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
            var rowExist = _context.SiteSettings.Any();

            // create a new instance of SiteSettings
            // if none exist.
            // This is needed to bypass null errors
            // during website creation.
            
            if (!rowExist)
            {
                SiteSettings onCreate = new SiteSettings()
                {
                    SiteName = "Site Name",
                    SiteDescription = "Site Description"
                };

                _context.Add(onCreate);
                _context.SaveChanges();
            }


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
