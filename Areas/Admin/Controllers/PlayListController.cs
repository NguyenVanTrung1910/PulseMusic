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
    public class PlayListController : Controller
    {
        private readonly PulseMusicContext _context;

        public PlayListController(PulseMusicContext context)
        {
            _context = context;
        }

        // GET: Admin/PlayList
        public async Task<IActionResult> Index()
        {
            var pulseMusicContext = _context.PlayLists.Include(p => p.Account);
            return View(await pulseMusicContext.ToListAsync());
        }

        // GET: Admin/PlayList/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.PlayLists == null)
            {
                return NotFound();
            }

            var playList = await _context.PlayLists
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playList == null)
            {
                return NotFound();
            }

            return View(playList);
        }

        // GET: Admin/PlayList/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            return View();
        }

        // POST: Admin/PlayList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AccountId")] PlayList playList)
        {
            if (ModelState.IsValid)
            {
                var lastId = _context.PlayLists.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                var nextId = lastId.Substring(0, 1) + (Convert.ToInt16(lastId.Substring(1)) + 1).ToString();
                playList.Id = nextId;
                _context.Add(playList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", playList.AccountId);
            return View(playList);
        }

        // GET: Admin/PlayList/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.PlayLists == null)
            {
                return NotFound();
            }

            var playList = await _context.PlayLists.FindAsync(id);
            if (playList == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", playList.AccountId);
            return View(playList);
        }

        // POST: Admin/PlayList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,AccountId")] PlayList playList)
        {
            if (id != playList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayListExists(playList.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", playList.AccountId);
            return View(playList);
        }

        // GET: Admin/PlayList/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.PlayLists == null)
            {
                return NotFound();
            }

            var playList = await _context.PlayLists
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playList == null)
            {
                return NotFound();
            }

            return View(playList);
        }

        // POST: Admin/PlayList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.PlayLists == null)
            {
                return Problem("Entity set 'PulseMusicContext.PlayLists'  is null.");
            }
            var playList = await _context.PlayLists.FindAsync(id);
            if (playList != null)
            {
                _context.PlayLists.Remove(playList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayListExists(string id)
        {
          return (_context.PlayLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
