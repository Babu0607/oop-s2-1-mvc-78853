using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Library.MVC.Services;  // Add this for SeedData

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Identity services - THIS IS THE IMPORTANT PART!
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        // Password settings
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

// seedData
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // Add this
app.UseAuthorization();   // Add this

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();  // Add this for Identity pages

app.Run();