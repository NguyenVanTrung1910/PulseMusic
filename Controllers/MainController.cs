using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseMusic.Models;

namespace PluseMusic.Controllers
{

    public class MainController : Controller
    {
        private readonly PulseMusicContext _context;
        public MainController(PulseMusicContext context)
        {
            _context = context;
        }
        public IActionResult Discover()
        {
            ViewBag.Tredding = _context.Musics.OrderByDescending(a => a.Player).Take(8).Include(a=>a.Artist).ToList();
			ViewBag.New = _context.Musics.OrderByDescending(a => a.PostingTime).Take(8).Include(a => a.Artist).ToList();
			ViewBag.Recommand = _context.Musics.Take(8).Include(a => a.Artist).ToList();
			return View();
        }
        public IActionResult Browse()
        {
            ViewBag.Filter = _context.Genres.ToList();
            return View(_context.Musics.Include(a => a.Genre).Include(a => a.Artist).ToList());
        }
        [HttpGet]
        public IActionResult FilterGenre(string genre)
        {
            ViewBag.Filter = _context.Genres.ToList();
            ViewBag.Genre = genre;
            if (genre.Equals("All"))
            {
                return View("Browse", _context.Musics.Include(a => a.Genre).Include(a => a.Artist).ToList());
            }
            return View("Browse", _context.Musics.Where(a => a.Genre.Name.Equals(genre)).Include(a=>a.Genre).Include(a=>a.Artist).ToList());
        }

        public IActionResult Charts()
        {
            return View(_context.Musics.OrderByDescending(a => a.Player).Include(a => a.Artist).Take(20).ToList());
        }
        public IActionResult Artist() { 
            ViewBag.SongCount = _context.Musics
            .GroupBy(m => m.ArtistId) // Nhóm các bài hát theo ArtistId
            .Select(g => new
            {
                ArtistId = g.Key,
                ArtistName = g.FirstOrDefault().Artist.Name, // Giả sử Artist có một thuộc tính Name
                SongCount = g.Count() // Đếm số lượng bài hát trong mỗi nhóm
            })
            .ToList();
            return View(_context.Artists.Take(12).ToList());
        }
        public IActionResult ArtistDetail(string id)
        {
            ViewBag.NumberAlbum = _context.Albums
                                  .Where(a => a.ArtistId.Equals(id))
                                  .Count();
            ViewBag.NumberTrack = _context.Tracks
                                  .Where(a => a.ArtistId.Equals(id))
                                  .Count();
            ViewBag.Popular = _context.Musics.Where(a => a.ArtistId.Equals(id)).OrderByDescending(a => a.Player).Include(a=>a.Artist).Take(4).ToList();
            ViewBag.Album = _context.Albums.Where(a => a.ArtistId.Equals(id)).ToList();
            ViewBag.AllMusic = _context.Musics.Include(a=>a.Artist).Take(20).ToList();

            var artist = _context.Artists.Where(a=>a.Id.Equals(id)).FirstOrDefault();
            if(artist != null)
            {
                return View(artist);
            }
            else
            {
                return View("Artist");
            }
            
        }
        
        public IActionResult AlbumDetail(string id)
        {
            ViewBag.PlayerOfAlbum = _context.Musics.Where(a=>a.AlbumId.Equals(id)).Sum(a=>a.Player);
            ViewBag.LikeOfAlbum = _context.Musics.Where(a => a.AlbumId.Equals(id)).Sum(a => a.Likes);
            ViewBag.Music = _context.Musics.Where(a => a.AlbumId.Equals(id)).Include(a=>a.Artist).ToList();
            ViewBag.OtherMusic = _context.Musics.Where(a => !a.AlbumId.Equals(id)).Include(a=>a.Artist).Take(4).ToList();
            return View(_context.Albums.Where(a=>a.Id.Equals(id)).Include(a=>a.Artist).FirstOrDefault());
        }
        public IActionResult Search()
        {
            return View(_context.Musics.Include(a=>a.Artist).Take(8).ToList());
        }
        [HttpPost]
        public IActionResult GetMusic(string searchkey)
        {
            var results = _context.Musics
                          .Where(a => a.Name.Contains(searchkey))
                          .Include(a => a.Artist)
                          .ToList();
            ViewBag.Number = _context.Musics
                          .Where(a => a.Name.Contains(searchkey))
                          .Count();
            ViewBag.Key = searchkey;
            ViewBag.Artist = _context.Artists
                              .Where(a => a.Name.Contains(searchkey))
                              .Include(a => a.Albums)
                              .ToList();
            ViewBag.SongCount = _context.Musics
            .GroupBy(m => m.ArtistId) // Nhóm các bài hát theo ArtistId
            .Select(g => new
            {
                ArtistId = g.Key,
                ArtistName = g.FirstOrDefault().Artist.Name, // Giả sử Artist có một thuộc tính Name
                SongCount = g.Count() // Đếm số lượng bài hát trong mỗi nhóm
            })
            .ToList();
            return View("Search",results);
        }
        

    }
}
