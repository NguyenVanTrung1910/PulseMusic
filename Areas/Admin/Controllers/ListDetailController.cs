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
    public class ListDetailController : Controller
    {
        private readonly PulseMusicContext _context;

        public ListDetailController(PulseMusicContext context)
        {
            _context = context;
        }

        // GET: Admin/ListDetail
        public async Task<IActionResult> Index()
        {
            var pulseMusicContext = _context.ListDetails.Include(l => l.Music).Include(l => l.PlayList);
            return View(await pulseMusicContext.ToListAsync());
        }

        // GET: Admin/ListDetail/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.ListDetails == null)
            {
                return NotFound();
            }

            var listDetail = await _context.ListDetails
                .Include(l => l.Music)
                .Include(l => l.PlayList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listDetail == null)
            {
                return NotFound();
            }

            return View(listDetail);
        }

        // GET: Admin/ListDetail/Create
        public IActionResult Create()
        {
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name");
            ViewData["PlayListId"] = new SelectList(_context.PlayLists, "Id", "Name");
            return View();
        }

        // POST: Admin/ListDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MusicId,PlayListId")] ListDetail listDetail)
        {
            if (listDetail!=null)
            {
                var lastId = _context.ListDetails.OrderByDescending(a => Convert.ToInt32(a.Id.Substring(2))).FirstOrDefault().Id;
                var nextId = lastId.Substring(0, 2) + (Convert.ToInt16(lastId.Substring(2)) + 1).ToString();
                listDetail.Id = nextId;
                _context.Add(listDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", listDetail.MusicId);
            ViewData["PlayListId"] = new SelectList(_context.PlayLists, "Id", "Name", listDetail.PlayListId);
            return View(listDetail);
        }

        // GET: Admin/ListDetail/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ListDetails == null)
            {
                return NotFound();
            }

            var listDetail = await _context.ListDetails.FindAsync(id);
            if (listDetail == null)
            {
                return NotFound();
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", listDetail.MusicId);
            ViewData["PlayListId"] = new SelectList(_context.PlayLists, "Id", "Name", listDetail.PlayListId);
            return View(listDetail);
        }

        // POST: Admin/ListDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,MusicId,PlayListId")] ListDetail listDetail)
        {
            if (id != listDetail.Id)
            {
                return NotFound();
            }

            if (listDetail!=null)
            {
                try
                {
                    _context.Update(listDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListDetailExists(listDetail.Id))
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
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", listDetail.MusicId);
            ViewData["PlayListId"] = new SelectList(_context.PlayLists, "Id", "Name", listDetail.PlayListId);
            return View(listDetail);
        }

        // GET: Admin/ListDetail/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ListDetails == null)
            {
                return NotFound();
            }

            var listDetail = await _context.ListDetails
                .Include(l => l.Music)
                .Include(l => l.PlayList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listDetail == null)
            {
                return NotFound();
            }

            return View(listDetail);
        }

        // POST: Admin/ListDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ListDetails == null)
            {
                return Problem("Entity set 'PulseMusicContext.ListDetails'  is null.");
            }
            var listDetail = await _context.ListDetails.FindAsync(id);
            if (listDetail != null)
            {
                _context.ListDetails.Remove(listDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListDetailExists(string id)
        {
          return (_context.ListDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
