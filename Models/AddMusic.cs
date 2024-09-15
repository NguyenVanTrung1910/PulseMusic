using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PulseMusic.Models
{
    public class AddMusic: ViewComponent
	{
		private readonly PulseMusicContext _context;
		public AddMusic(PulseMusicContext context)
		{
			_context = context;
		}
		public IViewComponentResult Invoke()
		{
			var id = HttpContext.Session.GetString("Id");
            return View(_context.PlayLists.Where(a=>a.AccountId.Equals(id)).ToList());
		}
	}
}
