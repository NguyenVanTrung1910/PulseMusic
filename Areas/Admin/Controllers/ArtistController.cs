using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PulseMusic.Models;

namespace PulseMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArtistController : Controller
    {
        private readonly PulseMusicContext _context;

        public ArtistController(PulseMusicContext context)
        {
            _context = context;
        }

        // GET: Admin/Artist
        public async Task<IActionResult> Index()
        {
              return _context.Artists != null ? 
                          View(await _context.Artists.ToListAsync()) :
                          Problem("Entity set 'PulseMusicContext.Artists'  is null.");
        }

        // GET: Admin/Artist/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Artists == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Admin/Artist/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Artist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image")] Artist artist, IFormFile ImageSpecial)
        {
            if (ModelState.IsValid)
            {
                var lastId = _context.Artists.OrderByDescending(a => Convert.ToInt32(a.Id.Substring(2))).FirstOrDefault().Id;
                var nextId = lastId.Substring(0, 2) + (Convert.ToInt16(lastId.Substring(2)) + 1).ToString();
                artist.Id = nextId;
				var fileName = Path.GetFileName(ImageSpecial.FileName);
                if(!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName))){
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageSpecial.CopyToAsync(fileStream);
                    }
                }
                artist.Image = fileName;
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Admin/Artist/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Artists == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Admin/Artist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Image")] Artist artist, IFormFile ImageSpecial)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var fileName = Path.GetFileName(ImageSpecial.FileName);
                    if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName)))
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageSpecial.CopyToAsync(fileStream);
                        }
                    }
                    artist.Image = fileName;
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
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
            return View(artist);
        }

        // GET: Admin/Artist/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Artists == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Admin/Artist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Artists == null)
            {
                return Problem("Entity set 'PulseMusicContext.Artists'  is null.");
            }
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(string id)
        {
          return (_context.Artists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
