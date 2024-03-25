namespace FpConsole;

public class Book
{
    public static Book GetDefault()
    {
        return new Book() { Title = "Not exist, this is a default book", Price = new BookPrice(0) { StartDate = DateOnly.FromDateTime(DateTime.Now) } };
    }

    public static Book GetDefault(string title) => new() { Title = title, Price = new BookPrice (0) { StartDate = DateOnly.FromDateTime(DateTime.Now) } };

    public int Id { get; set; }
    public string Title { get; set; }
    public string? Descriptions { get; set; }

    /// <summary>
    /// 不可以是 Nullable！
    /// System.InvalidOperationException: 'Configuring the complex property 'Book.Price' as optional is not supported, call 'IsRequired()'. See https://github.com/dotnet/efcore/issues/31376 for more information.'
    /// </summary>
    public required BookPrice Price { get; set; }

    public Aurthor Aurthor { get; set; }

    public List<Aurthor> CoAuthors { get; set; }

    public List<string> Comments { get; set; } = new();
}