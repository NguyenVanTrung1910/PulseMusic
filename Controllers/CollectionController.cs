using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseMusic.Models;

namespace PluseMusic.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class CollectionController : Controller
    {
        private readonly PulseMusicContext _context;
        public CollectionController(PulseMusicContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Tracks = _context.Tracks.Where(a => a.AccountId.Equals(HttpContext.Session.GetString("Id"))).Include(a => a.Artist).ToList();
            ViewBag.PlayList = _context.PlayLists.Where(a => a.AccountId.Equals(HttpContext.Session.GetString("Id"))).Include(a=>a.ListDetails).ToList();
            ViewBag.Liking = _context.Likes.Where(a => a.AccountId.Equals(HttpContext.Session.GetString("Id"))).Include(a => a.Music).Take(15).ToList();
            ViewBag.NumberPlaylist = _context.PlayLists.Where(a => a.AccountId.Equals(HttpContext.Session.GetString("Id"))).Count();
            ViewBag.NumberTrack = _context.Tracks.Where(a => a.AccountId.Equals(HttpContext.Session.GetString("Id"))).Count();
            ViewBag.AllListDetail = _context.ListDetails.Include(a=>a.Music).ToList();
            return View();
        }
        public IActionResult PlayList(string Id)
        {
            ViewBag.AllListDetail = _context.ListDetails.Where(a=>a.PlayListId.Equals(Id)).Include(a => a.Music).ToList();
            ViewBag.OtherMusic = _context.Musics.Include(a => a.Artist).Take(15).ToList();
            ViewBag.Liking = _context.Likes.Where(a => a.AccountId.Equals(HttpContext.Session.GetString("Id"))).Include(a=>a.Music).Take(15).ToList();
            return View(_context.PlayLists.Where(a=>a.Id.Equals(Id)).Include(a=>a.Account).FirstOrDefault());
        }
        [HttpPost]
        public IActionResult AddPlaylist(string playlist, string musicId)
        {
            if(_context.ListDetails.Where(a=>a.MusicId.Equals(musicId) && a.PlayListId.Equals(playlist)).Count()!= 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var nextdetailtid = "LD1";
            if (_context.ListDetails.Count() != 0)
            {
                var lastdetailid = _context.ListDetails.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                nextdetailtid = lastdetailid.Substring(0, 2) + (Convert.ToInt16(lastdetailid.Substring(2)) + 1).ToString();
            }
            
            var newListDetail = new ListDetail
            {
                Id = nextdetailtid,
                MusicId = musicId,
                PlayListId = playlist,
            };
            _context.ListDetails.Add(newListDetail);
            _context.SaveChanges();
            return RedirectToAction("Index","collection");
        }
        public IActionResult CreatePlayList(string name)
        {
            var id = HttpContext.Session.GetString("Id");
            if(id == null)
            {
                return View("Signin", "Account");
            }
            var nextId = "P1";
            if(_context.PlayLists.Count()!= 0)
            {
                var lastId = _context.PlayLists.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                nextId = lastId.Substring(0, 1) + (Convert.ToInt16(lastId.Substring(1)) + 1).ToString();
            }
            
            var newpl = new PlayList()
            {
                Id = nextId,
                Name = name,
                AccountId = id
            };
            _context.PlayLists.Add(newpl);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
		[HttpGet]
		
        public async Task<IActionResult> Like(string id)
        {
            
            
                var accountId = HttpContext.Session.GetString("Id");
                if (string.IsNullOrEmpty(accountId))
                {
                    return Json(new { success = false, message = "User is not logged in" });
                }

                var lastLike = await _context.Likes.OrderByDescending(p=> Convert.ToInt32(p.Id.Substring(2))).FirstOrDefaultAsync();
                var nextlikeid = "LK1";
                if (lastLike != null )
                {
                    var lastlikeid = lastLike.Id;
                    nextlikeid = lastlikeid.Substring(0, 2) + (Convert.ToInt16(lastlikeid.Substring(2)) + 1).ToString();
                }

                var like = new Like
                {
                    Id = nextlikeid,
                    AccountId = accountId,
                    MusicId = id,
                };

                var alreadyLiked = await _context.Likes.FirstOrDefaultAsync(a => a.AccountId.Equals(like.AccountId) && a.MusicId.Equals(like.MusicId));
                if (alreadyLiked == null)
                {
                    var music = _context.Musics.Where(a=>a.Id.Equals(like.MusicId)).FirstOrDefault();
                    music.Likes += 1;
                    _context.Musics.Update(music);
                    _context.Likes.Add(like);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Like added successfully", like = music.Likes});
                }

                return Json(new { success = false, message = "You have already liked this item" });
            
           
        }

        [HttpPost]
        public IActionResult DeleteArtist(string id)
        {
            var tracks = _context.Tracks.Where(a => a.ArtistId.Equals(id) && a.AccountId.Equals(HttpContext.Session.GetString("Id"))).FirstOrDefault();
            _context.Tracks.Remove(tracks);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteLike(string id)
        {
            var like = _context.Likes.Where(a => a.Id.Equals(id)).FirstOrDefault();
            var music = _context.Musics.Where(a => a.Id.Equals(like.MusicId)).FirstOrDefault();
            music.Likes -= 1;
            _context.Musics.Update(music);
            _context.Likes.Remove(like);
            _context.SaveChanges();
            return Json(new { success = true });
        }
        public IActionResult DeleteMusic(string id)
        {
            var listdetail = _context.ListDetails.Where(a=>a.Id.Equals(id)).FirstOrDefault();
            if(listdetail != null)
            {
                _context.ListDetails.Remove(listdetail);
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }
        public IActionResult Deleteplaylist(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Invalid playlist ID." });
                }

                var playlist = _context.PlayLists.FirstOrDefault(a => a.Id.Equals(id));
                if (playlist == null)
                {
                    return Json(new { success = false, message = "Playlist not found." });
                }

                _context.PlayLists.Remove(playlist);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Follow(string id)
        {
            try
            {
                var accountId = HttpContext.Session.GetString("Id");
                if (string.IsNullOrEmpty(accountId))
                {
                    return Json(new { success = false, message = "User is not logged in" });
                }

                var lastTrack = await _context.Tracks.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
                var nextTrackid = "LK1";
                if (lastTrack != null)
                {
                    var lastTrackId = lastTrack.Id;
                    nextTrackid = lastTrackId.Substring(0, 1) + (Convert.ToInt16(lastTrackId.Substring(1)) + 1).ToString();
                }

                var track = new Track
                {
                    Id = nextTrackid,
                    AccountId = accountId,
                    ArtistId = id,
                };

                var alreadyTrack = await _context.Tracks.FirstOrDefaultAsync(a => a.AccountId.Equals(track.AccountId) && a.ArtistId.Equals(track.ArtistId));
                if (alreadyTrack == null)
                {
                    _context.Tracks.Add(track);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Success!!" });
                }

                return Json(new { success = false, message = "You have already followed this artist" });
            }
            catch (DbUpdateException ex)
            {
                // Ghi log cho lỗi và cung cấp thông báo thân thiện hơn với người dùng
                return Json(new { success = false, message = "Đã xảy ra lỗi trong khi lưu lượt thích. Vui lòng thử lại sau." });
            }
            catch (Exception ex)
            {
                // Ghi log cho ngoại lệ (cân nhắc sử dụng một framework ghi log như Serilog, NLog, vv.)
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult AddPlayer(string id)
        {
            var music = _context.Musics.FirstOrDefault(a => a.Id.Equals(id));
            if (music != null)
            {
                // Tăng giá trị Player
                music.Player += 1;

                // Chỉ cần gọi SaveChanges để lưu thay đổi
                _context.SaveChanges();

                return Json(new { success = true });
            }

            // Trường hợp không tìm thấy bản ghi
            return Json(new { success = false, message = "Music not found" });
        }


    }
}
