using DorduncuDenemeKO.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DorduncuDenemeKO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secured()
        {
            string adres = "https://www.wired.com/category/backchannel/";
            WebRequest istek = HttpWebRequest.Create(adres);
            WebResponse cevap;
            cevap = istek.GetResponse();
            StreamReader streamReader = new StreamReader(cevap.GetResponseStream());
            string gelen = streamReader.ReadToEnd();
            string[] linkler = gelen.Split("/story/");
            // linkler2 aynı zamanda başlık
            string[] linkler2 = new string[15];
            string[] yazilar = new string[15];
            int k = 0;
            int i = 0;
            //Linklerin ayrıştırılması
            while (k < 6)
            {
                linkler2[k] = linkler[3 + i];
                linkler2[k] = linkler2[k].Substring(0, linkler2[k].IndexOf("/"));
                ViewData["baslik" + k] = linkler2[k];
                k = k + 1;
                i = i + 6;
            }
            for (int l = 1; l <6; l++)
            {
                WebClient client = new WebClient();
                var text = client.DownloadString("https://www.wired.com/story/" + linkler2[l] + "/");
                text = text.Substring(text.IndexOf("+++lead-in-text") + 19, text.IndexOf("&nbsp;"));
                yazilar[l - 1] = text;
                ViewData["yazi" + (l - 1)] = yazilar[l - 1];
            }

            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (username=="s" && password=="s")
            {
                var claims = new List<Claim>();
                claims.Add(new Claim("username", username));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                return Redirect(returnUrl);
            }
            TempData["Error"] = "Error. Username or password is invalid.";
            return View("login");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
