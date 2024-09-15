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
    public class TrackController : Controller
    {
        private readonly PulseMusicContext _context;

        public TrackController(PulseMusicContext context)
        {
            _context = context;
        }

        // GET: Admin/Track
        public async Task<IActionResult> Index()
        {
            var pulseMusicContext = _context.Tracks.Include(t => t.Account).Include(t => t.Artist);
            return View(await pulseMusicContext.ToListAsync());
        }

        // GET: Admin/Track/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Tracks == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks
                .Include(t => t.Account)
                .Include(t => t.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // GET: Admin/Track/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id");
            return View();
        }

        // POST: Admin/Track/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,ArtistId")] Track track)
        {
            if (track != null)
            {
                var lastId = _context.Tracks.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                var nextId = lastId.Substring(0, 1) + (Convert.ToInt16(lastId.Substring(1)) + 1).ToString();
                track.Id = nextId;
                _context.Add(track);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", track.AccountId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", track.ArtistId);
            return View(track);
        }

        // GET: Admin/Track/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Tracks == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", track.AccountId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", track.ArtistId);
            return View(track);
        }

        // POST: Admin/Track/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,AccountId,ArtistId")] Track track)
        {
            if (id != track.Id)
            {
                return NotFound();
            }

            if (track !=null)
            {
                try
                {
                    _context.Update(track);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackExists(track.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", track.AccountId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Id", track.ArtistId);
            return View(track);
        }

        // GET: Admin/Track/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Tracks == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks
                .Include(t => t.Account)
                .Include(t => t.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Admin/Track/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Tracks == null)
            {
                return Problem("Entity set 'PulseMusicContext.Tracks'  is null.");
            }
            var track = await _context.Tracks.FindAsync(id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackExists(string id)
        {
          return (_context.Tracks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
