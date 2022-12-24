using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public PostsController(ApplicationDbContext context, ImageService imageService, TagService tagService)
        {
            _context = context;
            _imageService = imageService;
            _tagService = tagService;
        }

        [Route("contactme")]
        public IActionResult ContactMe()
        {
            return View();
        }

        // GET: Posts
        public async Task<IActionResult> Index(string? s)
        {
            if(s == "cosplay")
            {
                var post = await _context.Post
                    .Where(p => p.Category == s)
                    .ToListAsync();

                return View(post);
            }
            if (s == "portrait")
            {
                var post = await _context.Post
                    .Where(p => p.Category == s)
                    .ToListAsync();

                return View(post);
            }


            return View(await _context.Post.ToListAsync());

        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Route("admin/")]
        public async Task<IActionResult> Create()
        {
            var posts = await _context.Post
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(posts);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("posts/create")]
        public async Task<IActionResult> Create([Bind("Id,Category,ImageFile,ModelName,InstagramUsername")] Post post, Tag tag)
        {
            if (ModelState.IsValid)
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
                return Redirect("~/admin");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,CreatedDate")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
            return Redirect("~/admin");
        }

        private bool PostExists(int id)
        {
          return (_context.Post?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
