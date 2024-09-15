using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PulseMusic.Models
{
    public class MusicLiking : ViewComponent
    {
        private readonly PulseMusicContext _context;
        public MusicLiking(PulseMusicContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Musics.OrderByDescending(a => a.Likes).Take(5).Include(a => a.Artist).ToList());
        }
    }
}
