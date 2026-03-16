using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Library.MVC.Services;  // Add this for SeedData

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddRoles<IdentityRole>()  // Add role services
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();   

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();