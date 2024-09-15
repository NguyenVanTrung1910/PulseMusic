using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PulseMusic.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Google;

namespace PulseMusic.Controllers
{    
    public class AccountController : Controller
    {
        private readonly PulseMusicContext _context;
        public AccountController(PulseMusicContext context)
        {
            _context = context;
        }
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signin([Bind("Email,Password")] Account acc)
        {
            var account = _context.Accounts.Where(a=>a.Email.Equals(acc.Email)&& a.Password.Equals(acc.Password)).FirstOrDefault();
            if(account != null)
            {
                HttpContext.Session.SetString("Id", account.Id);
                HttpContext.Session.SetString("Name", account.Name);
                HttpContext.Session.SetString("Email", account.Email);
                HttpContext.Session.SetString("Role", account.Role );
                var claims = new List<Claim>//tạo ra claim để chứa các thuộc tính của client
                {
                    new Claim(ClaimTypes.Email, account.Email ),
                    new Claim(ClaimTypes.Role, account.Role ),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );
                return RedirectToAction("Discover", "Main");
            }
            var currentaccount = new Account { Email = acc.Email, Password = acc.Password};
            ViewBag.Error = "Email or Password is correct";
            return View(currentaccount);
        }
        public IActionResult Signup()
        {
            return View();
        }
		public IActionResult LoginFacebook(string returnUrl = "/")
		{
			return Challenge(new AuthenticationProperties { RedirectUri = "/account/ExternalLoginCallback" }, "Facebook");
		}
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var userPrincipal = authenticateResult.Principal;

            // Get user information from the claims
            var email = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var name = userPrincipal.FindFirstValue(ClaimTypes.Name);

            // Kiểm tra xem tài khoản đã tồn tại trong database chưa (dựa trên email)
            var existingAccount = _context.Accounts.FirstOrDefault(p => p.Email == email);

            if (existingAccount == null)
            {
                var lastaccountid = _context.Accounts.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                var nextaccountid = lastaccountid.Substring(0, 1) + (Convert.ToInt16(lastaccountid.Substring(1)) + 1).ToString();
                // Nếu chưa có tài khoản, tạo tài khoản mới
                var newAccount = new Account
                {
                    Id = nextaccountid,
                    Email = email,
                    Name = name,
                    Password = "matkhaugia",
                    Role = "User"
                };
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                // Cập nhật thông tin session với tài khoản mới tạo
                HttpContext.Session.SetString("Id", newAccount.Id);
            }
            else
            {
                // Nếu tài khoản đã tồn tại, sử dụng tài khoản đó
                HttpContext.Session.SetString("Id", existingAccount.Id);
            }

            // Đặt thông tin vào session
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetString("Role", "User");

            // Tạo claim cho role và đăng nhập
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(userPrincipal.Identity, claims);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return LocalRedirect(returnUrl);
        }

        public IActionResult LoginGoogle(string returnUrl = "/")
        {
            var redirectUrl = Url.Action("ExternalLoginCallbackGoogle", "Account", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Callback sau khi Google xác thực thành công
        public async Task<IActionResult> ExternalLoginCallbackGoogle(string returnUrl = "/")
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var userPrincipal = authenticateResult.Principal;

            // Get user information from the claims
            var email = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var name = userPrincipal.FindFirstValue(ClaimTypes.Name);

            // Kiểm tra xem tài khoản đã tồn tại trong database chưa (dựa trên email)
            var existingAccount = _context.Accounts.FirstOrDefault(p => p.Email == email);

            if (existingAccount == null)
            {
                var lastaccountid = _context.Accounts.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                var nextaccountid = lastaccountid.Substring(0, 1) + (Convert.ToInt16(lastaccountid.Substring(1)) + 1).ToString();
                // Nếu chưa có tài khoản, tạo tài khoản mới
                var newAccount = new Account
                {
                    Id = nextaccountid,
                    Email = email,
                    Name = name,
                    Password = "matkhaugia",
                    Role = "User"
                };
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                // Cập nhật thông tin session với tài khoản mới tạo
                HttpContext.Session.SetString("Id", newAccount.Id);
            }
            else
            {
                // Nếu tài khoản đã tồn tại, sử dụng tài khoản đó
                HttpContext.Session.SetString("Id", existingAccount.Id);
            }

            // Đặt thông tin vào session
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetString("Role", "User");

            // Tạo claim cho role và đăng nhập
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(userPrincipal.Identity, claims);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return LocalRedirect(returnUrl);
        }


        [HttpPost]
        public IActionResult Signup([Bind("Name,Email,Password")] Account account, string confirmpassword)
        {
            foreach (var a in _context.Accounts.ToList())
            {
                if (a.Email == account.Email)
                {
                    ViewBag.EmailError = "Email already exists";
                    return View(account);
                }
            }
            if (account.Password.Equals(confirmpassword))
            {
                var lastaccountid = _context.Accounts.OrderByDescending(a => a.Id).FirstOrDefault().Id;
                var nextaccountid = lastaccountid.Substring(0, 1) + (Convert.ToInt16(lastaccountid.Substring(1)) + 1).ToString();
                var accountsave = new Account { Email = account.Email, Password = account.Password, Name = account.Name, Role = "user",Id = nextaccountid};
                _context.Accounts.Add(accountsave);
                _context.SaveChanges();
                return View("Signin");
            }
            ViewBag.PasswordError = "Password does not match Confirmpassword";
            return View(account);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CheckEmail([Bind("Email")] Account account)
        {
            foreach (var a in _context.Accounts.ToList())
            {
                if (a.Email == account.Email)
                {
                    return View("ChangePassword",a);
                }
            }
            ViewBag.EmailError = "Email wasn't already exists";
            return View(account);
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword([Bind("Email,Password")] Account account,string newpassword, string confirmpassword)
        {
            var oldaccount = _context.Accounts.Where(a=>a.Email.Equals(account.Email)&&a.Password.Equals(account.Password)).FirstOrDefault();
            if (oldaccount!=null && newpassword.Equals(confirmpassword)&& newpassword.Length>=8&&newpassword.Length<=30)
            {
                oldaccount.Password = confirmpassword;
                _context.Accounts.Update(oldaccount);
                _context.SaveChanges();
                HttpContext.Session.SetString("Id", oldaccount.Id);
                HttpContext.Session.SetString("Name", oldaccount.Name);
                HttpContext.Session.SetString("Email", oldaccount.Email);
                HttpContext.Session.SetString("Role", oldaccount.Role);
                var claims = new List<Claim>//tạo ra claim để chứa các thuộc tính của client
                {
                    new Claim(ClaimTypes.Email, oldaccount.Email ),
                    new Claim(ClaimTypes.Role, oldaccount.Role ),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );
                return RedirectToAction("Discover", "Main");
            }
            return View(account);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Role");
            HttpContext.SignOutAsync();
            return View("Signin");
        }
    }
}
