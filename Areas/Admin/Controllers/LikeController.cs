using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PulseMusic.Models;

namespace PulseMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LikeController : Controller
    {
        private readonly PulseMusicContext _context;

        public LikeController(PulseMusicContext context)
        {
            _context = context;
        }

        // GET: Admin/Like
        public async Task<IActionResult> Index()
        {
            var pulseMusicContext = _context.Likes.Include(l => l.Account).Include(l => l.Music);
            return View(await pulseMusicContext.ToListAsync());
        }

        // GET: Admin/Like/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Likes == null)
            {
                return NotFound();
            }

            var like = await _context.Likes
                .Include(l => l.Account)
                .Include(l => l.Music)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (like == null)
            {
                return NotFound();
            }

            return View(like);
        }

        // GET: Admin/Like/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name");
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name");
            return View();
        }

        // POST: Admin/Like/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MusicId,AccountId")] Like like)
        {
            if (like!=null)
            {
                var lastId = _context.Likes.OrderByDescending(p=>Convert.ToInt32(p.Id.Substring(2))).FirstOrDefault().Id;
                var nextId = lastId.Substring(0, 2) + (Convert.ToInt16(lastId.Substring(2)) + 1).ToString();
                like.Id = nextId;
                _context.Add(like);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", like.AccountId);
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", like.MusicId);
            return View(like);
        }

        // GET: Admin/Like/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Likes == null)
            {
                return NotFound();
            }

            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", like.AccountId);
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", like.MusicId);
            return View(like);
        }

        // POST: Admin/Like/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,MusicId,AccountId")] Like like)
        {
            if (id != like.Id)
            {
                return NotFound();
            }

            if (like!=null)
            {
                try
                {
                    _context.Update(like);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LikeExists(like.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", like.AccountId);
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", like.MusicId);
            return View(like);
        }

        // GET: Admin/Like/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Likes == null)
            {
                return NotFound();
            }

            var like = await _context.Likes
                .Include(l => l.Account)
                .Include(l => l.Music)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (like == null)
            {
                return NotFound();
            }

            return View(like);
        }

        // POST: Admin/Like/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Likes == null)
            {
                return Problem("Entity set 'PulseMusicContext.Likes'  is null.");
            }
            var like = await _context.Likes.FindAsync(id);
            if (like != null)
            {
                _context.Likes.Remove(like);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LikeExists(string id)
        {
          return (_context.Likes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
