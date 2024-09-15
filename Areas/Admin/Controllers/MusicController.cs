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
    public class MusicController : Controller
    {
        private readonly PulseMusicContext _context;

        public MusicController(PulseMusicContext context)
        {
            _context = context;
        }

        // GET: Admin/Music
        public async Task<IActionResult> Index()
        {
            var pulseMusicContext = _context.Musics.Include(m => m.Album).Include(m => m.Artist).Include(m => m.Genre);
            return View(await pulseMusicContext.ToListAsync());
        }

        // GET: Admin/Music/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Musics == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .Include(m => m.Album)
                .Include(m => m.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            return View(music);
        }

        // GET: Admin/Music/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name");
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Admin/Music/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LinkAudio,Player,Likes,Image,PostingTime,AlbumId,ArtistId,GenreId")] Music music, IFormFile imageMusic, IFormFile audioMusic)
        {
            if (ModelState.IsValid)
            {
                var lastId = _context.Musics.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                var nextId = lastId.Substring(0, 1) + (Convert.ToInt16(lastId.Substring(1)) + 1).ToString();
                music.Id = nextId;
                var nameImage = Path.GetFileName(imageMusic.FileName);
                var nameAudio = Path.GetFileName(audioMusic.FileName);
                music.LinkAudio = nameAudio;
                music.Image = nameImage;
                if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", nameImage))){
                    var filePathMusic = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",nameImage);
                    using (var fileStream1 = new FileStream(filePathMusic, FileMode.Create))
                    {
                        await imageMusic.CopyToAsync(fileStream1);
                    }
                }
                if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\music", nameAudio))){
                    var filePathAudio = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\music", nameAudio);
                    using (var fileStream = new FileStream(filePathAudio, FileMode.Create))
                    {
                        await audioMusic.CopyToAsync(fileStream);
                    }
                }
                _context.Add(music);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", music.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", music.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", music.GenreId);

            return View(music);
        }

        // GET: Admin/Music/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Musics == null)
            {
                return NotFound();
            }

            var music = await _context.Musics.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", music.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", music.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", music.GenreId);

            return View(music);
        }

        // POST: Admin/Music/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,LinkAudio,Player,Likes,Image,PostingTime,AlbumId,ArtistId,GenreId")] Music music, IFormFile imageMusic, IFormFile audioMusic)
        {
            if (id != music.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var nameImage = Path.GetFileName(imageMusic.FileName);
                    var nameAudio = Path.GetFileName(audioMusic.FileName);
                    music.LinkAudio = nameAudio;
                    music.Image = nameImage;
                    if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", nameImage)))
                    {
                        var filePathMusic = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", nameImage);
                        using (var fileStream1 = new FileStream(filePathMusic, FileMode.Create))
                        {
                            await imageMusic.CopyToAsync(fileStream1);
                        }
                    }
                    if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\music", nameAudio)))
                    {
                        var filePathAudio = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\music", nameAudio);
                        using (var fileStream = new FileStream(filePathAudio, FileMode.Create))
                        {
                            await audioMusic.CopyToAsync(fileStream);
                        }
                    }
                    _context.Update(music);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicExists(music.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", music.AlbumId);
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", music.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", music.GenreId);

            return View(music);
        }

        // GET: Admin/Music/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Musics == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .Include(m => m.Album)
                .Include(m => m.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            return View(music);
        }

        // POST: Admin/Music/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Musics == null)
            {
                return Problem("Entity set 'PulseMusicContext.Musics'  is null.");
            }
            var music = await _context.Musics.FindAsync(id);
            if (music != null)
            {
                _context.Musics.Remove(music);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicExists(string id)
        {
          return (_context.Musics?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
