using CSharpFunctionalExtensions;

namespace FpConsole;

public class BookPrice : ValueObject
{
    public BookPrice() { }
    public BookPrice(decimal price) { Price = price; }

    public decimal Price { get; private set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public DateTime? ChangedDate { get; set; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Price;
        yield return StartDate;
    }

    public void Add(decimal value)
    {
        Price += value;
        ChangedDate = DateTime.Now;
    }

    public void Set(decimal value)
    {
        Price = value;
        ChangedDate = DateTime.UtcNow;
    }
}