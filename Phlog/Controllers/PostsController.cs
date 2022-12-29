using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Phlog.Data;
using Phlog.Models;
using Phlog.Services;

// todo return to top

namespace Phlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageService _imageService;
        private readonly TagService _tagService;
        private readonly UserManager<SiteOwner> _userManager;

        public PostsController(ApplicationDbContext context, ImageService imageService, TagService tagService, UserManager<SiteOwner> userManager)
        {
            _context = context;
            _imageService = imageService;
            _tagService = tagService;
            _userManager = userManager;
        }

        [Route("contactme")]
        public IActionResult ContactMe()
        {
			var siteOwner = _userManager.Users.FirstOrDefault();

            // redirect if no user has been registered
            // this is needed to fix null errors
            if (siteOwner == null)
            {
                return Redirect("~/Identity/Account/Register");
            }

			return View(siteOwner);
        }

        // GET: Posts
        public async Task<IActionResult> Index(string? s, string? c, string? m, string? tag)
        {
            // replace variables for easier reading
            string categoryName = c;
            string modelName = m;

            // if (c == "searchTerm") will give technical debt
            // resolve with (categoryName == categoryName)
            if (categoryName != null && categoryName == categoryName)
            {
                var post = await _context.Post
                    .Where(p => p.Category.Name.ToLower() == categoryName)
                    .ToListAsync();

                return View(post);
            }            
            if (modelName != null && modelName == modelName)
            {
                var post = await _context.Post
                    .Where(p => p.ModelName.ToLower() == modelName)
                    .ToListAsync();                
                
               return View(post);
            }


            if(tag != null)
            {
                var post = await _context.Post
                    .Where(p => p.Tags.Any(t => t.Name.Contains(tag)))
                    .ToListAsync();

                return View(post);
            }


            return View(await _context.Post.ToListAsync());

        }


        private bool PostExists(int id)
        {
          return (_context.Post?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
