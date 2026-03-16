using Microsoft.AspNetCore.Mvc;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers;

public class BooksController: Controller
{
    private readonly ApplicationDbContext _context;
    
    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index(string searchString, string category, string availability)
    {
        
        var booksQuery = _context.Books.AsQueryable();
        
        
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();
            booksQuery = booksQuery.Where(b => 
                b.Title.Contains(searchString) || 
                b.Author.Contains(searchString));
        }
        
       
        if (!string.IsNullOrWhiteSpace(category) && category != "All")
        {
            booksQuery = booksQuery.Where(b => b.Category == category);
        }
        
        
        if (!string.IsNullOrWhiteSpace(availability) && availability != "All")
        {
            if (availability == "Available")
            {
                booksQuery = booksQuery.Where(b => b.IsAvailable == true);
            }
            else if (availability == "OnLoan")
            {
                booksQuery = booksQuery.Where(b => b.IsAvailable == false);
            }
        }
        
        
        var categories = await _context.Books
            .Select(b => b.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
        
        
        var books = await booksQuery
            .OrderBy(b => b.Title)
            .ToListAsync();
        
        
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentCategory = category ?? "All";
        ViewBag.CurrentAvailability = availability ?? "All";
        ViewBag.Categories = categories;
        
        return View(books);
    }

    
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);

        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }


    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            book.IsAvailable = true;
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}