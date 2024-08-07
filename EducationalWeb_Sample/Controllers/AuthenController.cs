using Application;
using Domain;
using EducationalWeb_Sample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Xml.Linq;

namespace EducationalWeb_Sample.Controllers
{
    [Route("User")]
    public class AuthenController : Controller
    {
        private Users usersApp;
        private IMemoryCache memoryCache;

        public AuthenController(Users usersApp, IMemoryCache memoryCache)
        {
            this.usersApp = usersApp;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        private static List<Claim> GetRoles(string roleString)
        {
            var claimList = new List<Claim>();
            var rules = roleString.Split(',');
            foreach (string r in rules)
            {
                string role = r[..r.IndexOf('[')];

                claimList.Add(new Claim(ClaimTypes.Role, role));

                var claims = r[(r.IndexOf('[')+1)..r.IndexOf(']')];

                claimList.AddRange(claims.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(claim => new Claim($"{ClaimTypes.AuthorizationDecision}/{role}", claim)));


            }
            return claimList;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return View("index", user);
            }
            try
            {
                UserItem newUser = new UserItem()
                {
                    UserName = user.UserName,
                    Password = user.Password
                };

                var loggedIn = await usersApp.Login(newUser) ?? throw new ArgumentException("User or Password wrong");

               // var rules = loggedIn.Role.Split(',', StringSplitOptions.RemoveEmptyEntries);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, newUser.UserName)
                };
                claims.AddRange(GetRoles(loggedIn.Role));


                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) {

                ViewBag.LoggedIn = true;
                return View("index", null);

            }
        }

        [HttpPost("RegisterNew")]
        public async Task<IActionResult> RegisterNew(UserRegisterModel user)
        {
            if (!ModelState.IsValid || user.ReTypePassword != user.Password)
            {
                ViewBag.ErrorMessagePassword = "The Retyped password must be the same as the original password";
                return View("register", user);
            }

            var a = await usersApp.GetItemsByColumn("userName", user.UserName);

            if (a.Count > 0)
            {
                ViewBag.ErrorMessageUserName = "That Username already exists";
                return View("register");
            }

            UserItem newUser = new UserItem()
            {
                UserName = user.UserName,
                Password = user.Password,
            };


            await usersApp.InsertData(newUser);

            return RedirectToAction("Index", "User");
        }
               

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            ViewBag.IsLoggedOut = true;

            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            this.HttpContext.User = null;

            return View("index", null);
        }
    }
}
