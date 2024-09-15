using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration;
using NuGet.ContentModel;
using PulseMusic.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PulseMusicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PulseMusicContext") ?? throw new InvalidOperationException("Connection string 'PulseMusicContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication().AddFacebook(opt =>
{
	opt.ClientId = "869273135163903";
	opt.ClientSecret = "645e7f6b6958f753550599a9839b532e";
});
builder.Services.AddAuthentication()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "607379803462-p2g0urt2sqhke5gsk0bk9dgihqr6jefn.apps.googleusercontent.com";       // Google Client ID
    googleOptions.ClientSecret = "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"; // Google Client Secret
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//xác th?c ???c th?c thi b?ng cookie Authentication
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Signin";//khi m?t ng??i truy c?p vào ph?n không ???c phép s? t? ??ng h??ng ??n trang này
        option.ExpireTimeSpan = TimeSpan.FromMinutes(100); //th?i gian t?n t?i c?a cookie
    }
);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Main/Discover");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Discover}/{id?}");

app.Run();
