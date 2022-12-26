using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phlog.Data;
using Phlog.Models;

namespace Phlog.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context) 
        {
            _context = context;
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
