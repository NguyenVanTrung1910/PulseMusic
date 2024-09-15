using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PulseMusic.Models
{
    public class SearchMusic : ViewComponent
    {
        private readonly PulseMusicContext _context;
        public SearchMusic(PulseMusicContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Musics.Take(8).Include(a=>a.Artist).ToList());
        }
    }
}
