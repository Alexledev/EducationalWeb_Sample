using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EducationalWeb_Sample
{
    public class Authen
    {
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
