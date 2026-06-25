
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

public class BooksController : Controller
{
    private readonly MySqlDbContext _context;

    public BooksController(MySqlDbContext context)
    {
        _context = context;
    }

    // GET: BOOKS
    public async Task<IActionResult> Index()    
    {
        // EF DB books 테이블 SELECT 실행해서 데이터 가져옴
        var model = await _context.Books.ToListAsync();
        return View(model);
    }

    // GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? bookid)
    {
        if (bookid == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.BookId == bookid);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: BOOKS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BOOKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookId,Author,DivCode,BookNamer,ReleaseDt,ISBN,Price")] Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book); // INSERT 쿼리 실행
            await _context.SaveChangesAsync();  // COMMIT
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: BOOKS/Edit/5
    public async Task<IActionResult> Edit(int? bookid)
    {
        if (bookid == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(bookid);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: BOOKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? bookid, [Bind("BookId,Author,DivCode,BookNamer,ReleaseDt,ISBN,Price")] Book book)
    {
        if (bookid != book.BookId)
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
                if (!BookExists(book.BookId))
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

    // GET: BOOKS/Delete/5
    public async Task<IActionResult> Delete(int? bookid)
    {
        if (bookid == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.BookId == bookid);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: BOOKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? bookid)
    {
        var book = await _context.Books.FindAsync(bookid);
        if (book != null)
        {
            _context.Books.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? bookid)
    {
        return _context.Books.Any(e => e.BookId == bookid);
    }
}
