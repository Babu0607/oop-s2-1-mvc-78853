using Microsoft.AspNetCore.Identity;
using Library.Domain;
using Library.MVC.Data;
using Bogus;

namespace Library.MVC.Services;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

       
        string[] roleNames = { "Admin", "Staff" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        
        string adminEmail = "Billie@library1.com";
        string adminPassword = "Billie@123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

       
        if (context.Books.Any())
        {
            return; 
        }

        var bookFaker = new Faker<Book>()
            .RuleFor(b => b.Title, f => f.Commerce.ProductName())
            .RuleFor(b => b.Author, f => f.Name.FullName())
            .RuleFor(b => b.Isbn, f => f.Commerce.Ean13())
            .RuleFor(b => b.Category, f => f.PickRandom(new[] { 
                "Fiction", "Non-Fiction", "Science", "History", 
                "Biography", "Children", "Technology", "Art", 
                "Philosophy", "Poetry" 
            }))
            .RuleFor(b => b.IsAvailable, f => f.Random.Bool(0.7f)); // 70% available

        var books = bookFaker.Generate(20);
        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        
        var memberFaker = new Faker<Member>()
            .RuleFor(m => m.FullName, f => f.Name.FullName())
            .RuleFor(m => m.Email, f => f.Internet.Email())
            .RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber());

        var members = memberFaker.Generate(10);
        await context.Members.AddRangeAsync(members);
        await context.SaveChangesAsync();

        
        var loanFaker = new Faker<Loan>()
            .RuleFor(l => l.BookId, f => f.PickRandom(books).Id)
            .RuleFor(l => l.MemberId, f => f.PickRandom(members).Id)
            .RuleFor(l => l.LoanDate, f => f.Date.Past(60)) // Random date in last 60 days
            .RuleFor(l => l.DueDate, (f, l) => l.LoanDate.AddDays(14))
            .RuleFor(l => l.ReturnedDate, (f, l) => 
                f.Random.Bool(0.4f) ? l.LoanDate.AddDays(f.Random.Int(1, 13)) : (DateTime?)null);

        var loans = loanFaker.Generate(15);

       
        var activeLoans = loans.Where(l => !l.ReturnedDate.HasValue)
            .GroupBy(l => l.BookId)
            .Select(g => g.First())
            .ToList();

        var returnedLoans = loans.Where(l => l.ReturnedDate.HasValue).ToList();
        
        var finalLoans = returnedLoans.Concat(activeLoans).ToList();

        await context.Loans.AddRangeAsync(finalLoans);
        await context.SaveChangesAsync();

        
        foreach (var loan in activeLoans)
        {
            var book = books.First(b => b.Id == loan.BookId);
            book.IsAvailable = false;
        }
        await context.SaveChangesAsync();
    }
}