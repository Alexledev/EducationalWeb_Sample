using Application;
using EducationalWeb_Sample;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMySQLConnection();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Courses>();
builder.Services.AddSingleton<Blogs>();
builder.Services.AddSingleton<Users>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "_educaweb.auth";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.LoginPath = "/user";
    options.LogoutPath = "/user/logout";
});



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("BlogAdmin", "CourseAdmin"));
    options.AddPolicy("CanReadBlog", policy => policy.RequireClaim(Authen.GetClaimType("BlogAdmin"), "Read"));
    options.AddPolicy("CanEditBlog", policy => policy.RequireClaim(Authen.GetClaimType("BlogAdmin"), "Edit"));
    options.AddPolicy("CanCreateBlog", policy => policy.RequireClaim(Authen.GetClaimType("BlogAdmin"), "Create"));
    options.AddPolicy("CanDeleteBlog", policy => policy.RequireClaim(Authen.GetClaimType("BlogAdmin"), "Delete"));
    options.AddPolicy("CanEditOrCreateBlog", policy => policy.RequireClaim(Authen.GetClaimType("BlogAdmin"), "Create", "Edit"));

    options.AddPolicy("CanReadCourse", policy => policy.RequireClaim(Authen.GetClaimType("CourseAdmin"), "Read"));
    options.AddPolicy("CanEditCourse", policy => policy.RequireClaim(Authen.GetClaimType("CourseAdmin"), "Edit"));
    options.AddPolicy("CanCreateCourse", policy => policy.RequireClaim(Authen.GetClaimType("CourseAdmin"), "Create"));
    options.AddPolicy("CanDeleteCourse", policy => policy.RequireClaim(Authen.GetClaimType("CourseAdmin"), "Delete"));
    options.AddPolicy("CanEditOrCreateCourse", policy => policy.RequireClaim(Authen.GetClaimType("CourseAdmin"), "Create", "Edit"));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseMiddleware<Authen>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
