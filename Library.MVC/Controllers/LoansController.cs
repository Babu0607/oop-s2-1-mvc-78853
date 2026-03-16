using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers;

public class LoansController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoansController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
        
        return View(loans);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.BookId = new SelectList(
            await _context.Books.Where(b => b.IsAvailable).ToListAsync(),
            "Id", "Title");
        
        ViewBag.MemberId = new SelectList(
            await _context.Members.ToListAsync(),
            "Id", "FullName");
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Loan loan)
    {
        var book = await _context.Books.FindAsync(loan.BookId);
        if (book == null || !book.IsAvailable)
        {
            ModelState.AddModelError("BookId", "This book is not available for loan.");
        }

        if (ModelState.IsValid)
        {
            loan.LoanDate = DateTime.Today;
            loan.DueDate = DateTime.Today.AddDays(14);
            
            if (book != null) book.IsAvailable = false;
            
            _context.Add(loan);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        
        ViewBag.BookId = new SelectList(
            await _context.Books.Where(b => b.IsAvailable).ToListAsync(),
            "Id", "Title", loan.BookId);
        ViewBag.MemberId = new SelectList(
            await _context.Members.ToListAsync(),
            "Id", "FullName", loan.MemberId);
        
        return View(loan);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);
        
        if (loan == null)
        {
            return NotFound();
        }

        if (loan.ReturnedDate == null)
        {
            loan.ReturnedDate = DateTime.Today;
            if (loan.Book != null) loan.Book.IsAvailable = true;
            
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> Overdue()
    {
        var overdueLoans = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.ReturnedDate == null && l.DueDate < DateTime.Today)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
        
        return View(overdueLoans);
    }

    private bool LoanExists(int id)
    {
        return _context.Loans.Any(e => e.Id == id);
    }
}