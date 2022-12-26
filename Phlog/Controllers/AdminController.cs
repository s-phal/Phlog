using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phlog.Data;

namespace Phlog.Controllers
{
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

        [Route("admin/sitesettings")]
        public async Task<IActionResult> SiteSettings()
        {
            return View();
        }
    }
}
