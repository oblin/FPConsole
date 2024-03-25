using FpConsole;
using CSharpFunctionalExtensions;

namespace FPConsole.Services;

public class BookService
{
    private readonly DemoContext _context;

    public BookService(DemoContext context)
    {
        _context = context;
    }

    // CQS: Command that will not Fail
    public void AddOne(string title, decimal price)
    {
        var bookPrice = new BookPrice(price) { StartDate = DateOnly.FromDateTime(DateTime.Now)};
        var book = new Book { Title = title, Price = bookPrice };

        _context.Books.Add(book);
        _context.SaveChanges();
    }

    // CQS: Query that will not Fail
    public IEnumerable<Book> GetAll()
    {
        return _context.Books;
    }

    // CQS: Query that will failed
    public Maybe<Book> Get(int id)
    {
        // Implicit Convert to Maybe Type
        //return _context.Books.Find(id);
        return _context.Books.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Maybe<Book>> GetAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    // CQS: Command that will fail
    public Result Update(int id, decimal dPrice)
    {
        Maybe<Book> bookResult = _context.Books.Find(id);
        if (bookResult.HasValue) 
        {
            var book = bookResult.Value;
            book.Price.StartDate = DateOnly.FromDateTime(DateTime.Now);
            book.Price.Set(dPrice);
            _context.SaveChanges();
            return Result.Success();
        }

        return Result.Failure($"Not found {id}");
    }

    public void Update(Book book)
    {
        _context.Books.Update(book);
        _context.SaveChanges();
    }
}
