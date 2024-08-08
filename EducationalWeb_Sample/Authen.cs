using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EducationalWeb_Sample
{
    internal static class CookieExtensions
    {
        internal static void AddAuthInfo(this IResponseCookies cookies, IMemoryCache memoryCache, string userName, string GUID = null, int expireMinutes = 30)
        {
            string guid = GUID ?? Guid.NewGuid().ToString();

            var expireTime = DateTimeOffset.UtcNow.AddMinutes(expireMinutes);

            cookies.Append("Auth", guid, new CookieOptions
            {
                Expires = expireTime,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,

            });

            memoryCache.Set(guid, userName, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(expireMinutes)
            });
        }

        internal static bool VerifyAuthInfo(this HttpContext context, IMemoryCache memoryCache)
        {
            var cookies = context.Request.Cookies;
            var cookie = cookies.SingleOrDefault(c => c.Key == "Auth");

            if (cookie.Key != null && memoryCache.TryGetValue(cookie.Value, out string userName))
            {
                context.Response.Cookies.AddAuthInfo(memoryCache, userName, GUID: cookie.Value);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName!)
                };

                var newClaimsIdentity = new ClaimsIdentity(claims, "Authentication");

                context.User = new ClaimsPrincipal(newClaimsIdentity);
                Thread.CurrentPrincipal = context.User;
                //

                return true;
            }

            return false;

        }

        internal static void RemoveAuthInfo(this HttpContext context, IMemoryCache memoryCache)
        {
            var cookies = context.Request.Cookies;
            var cookie = cookies.SingleOrDefault(c => c.Key == "Auth");

            if (cookie.Key != null && memoryCache.TryGetValue(cookie.Value, out string value))
            {
                memoryCache.Remove(cookie.Value);
            }

            context.Response.Cookies.Delete("Auth");

            context.User = null;
            Thread.CurrentPrincipal = null;
        }
    }

    public class Authen
    {
        public static string GetClaimType(string name)
        {
            string n = $"{ClaimTypes.AuthorizationDecision}/{name}";
            return n;
        }

        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public Authen(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            //if (!context.VerifyAuthInfo(_memoryCache) && (context.Request.Path.StartsWithSegments("/admin") || context.Request.Path.StartsWithSegments("/content")))
            //{
            //    context.Response.Redirect("/authen/index");
            //    return;
            //}

            //var claims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, "Alex")
            //    };

            //var newClaimsIdentity = new ClaimsIdentity(claims, "Authentication");

            //context.User = new ClaimsPrincipal(newClaimsIdentity);
            //Thread.CurrentPrincipal = context.User;


            await _next(context);
        }
    }
}
