using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using FpConsole;
using FPConsole.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var services = new ServiceCollection();
var connectString = config["ConnectionStrings:demo"];
services.AddDbContext<DemoContext>(config => config.UseNpgsql(connectString));
services.AddScoped<BookService>();

var provider = services.BuildServiceProvider();

Console.WriteLine("Hello, World!");

bool exit = false;
while (!exit)
{
    Console.WriteLine(" 0. 離開：");
    Console.WriteLine(" 1. 新增一本書：");
    Console.WriteLine(" 2. (測試Maybe Type)查詢現有存書：");
    Console.WriteLine(" 3. (測試Maybe Type)查詢指定存書：");
    Console.WriteLine(" 4. (測試 Result)修改現有存書價格：");
    Console.WriteLine(" 5. (測試 ValueObject)修改現有存書價格：");
    Console.WriteLine(" 6. (測試 Array)修改現有存書評論：");

    var action = Console.ReadLine();
    if (!int.TryParse(action, out int iAction))
        continue;

    int iId = 0;
    decimal dPrice = 0;
    var bookService = provider.GetRequiredService<BookService>();
    switch (iAction)
    {
        case 1:
            Console.Write(" 輸入書名：");
            var title = Console.ReadLine();
            Console.Write(" 輸入價格：");
            var price = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title) || !decimal.TryParse(price, out dPrice))
            {
                Console.WriteLine("輸入的書名與價格錯誤，請重新輸入");
            }
            else
            {
                bookService.AddOne(title, dPrice);
            }
            break;
        case 2:
            foreach (Book book in bookService.GetAll())
                Console.WriteLine($"Id：{book.Id} Title: {book.Title}: Price: {book.Price.Price} from {book.Price.StartDate}");
            // IEnumerable 可以直接轉換成 Maybe Type 
            bookService.GetAll().TryFirst()
                .Execute(book => Console.WriteLine($"Execute: first book is: {book.Title}"));

            bookService.GetAll().TryFirst(b => b.Id == 0)
                .ExecuteNoValue(() => Console.WriteLine("ExecuteNoValue: No id 0 Item"));

            break;
        case 3:
            Console.Write(" 輸入 Id：");
            string? id = Console.ReadLine();
            if (int.TryParse(id, out iId))
            {
                // Implicit Convert to Maybe Type 
                var oneBook = bookService.Get(iId);

                if (oneBook.HasValue && (oneBook == oneBook.Value))
                    // 可以直接比較 Maybes or values
                    Console.WriteLine("Maybe<Book> == Book");

                var noBook = Maybe<Book>.None;
                // Or: if no value, execute
                Console.WriteLine("Or: " + noBook.Or(oneBook.Or(() => Book.GetDefault())).ToString());

                if (oneBook.HasValue)
                {
                    Console.WriteLine($"book title: {oneBook.Value.Title}, value is: " + oneBook.GetValueOrDefault().Price.Price);
                    var secBook = bookService.Get(2);
                    // Maybe Type can compare equality
                    if (oneBook == secBook)
                        Console.WriteLine("You pick id = 2 item");

                    // Where condition return Maybe book:  Converting a Maybe with a value to a Maybe.None if a condition isn't met
                    var idIs3 = oneBook.Where(p => p.Id == 3);
                    // GetValueOrDefault 可以指定如果找不到，所使用預設的值
                    Console.WriteLine("Where example: Id 3 Title is:" + idIs3.GetValueOrDefault(Book.GetDefault()).Title);

                    // Map (Select) 轉換型態： Maybe<T> => Maybe<V>, 或這 Maybe<T> => Result<T> Map 只有當有value 時候才會進入，回傳的結果會改變型態
                    Console.WriteLine($"Your choice is Id == 4 ? {oneBook.Select(o => o.Id == 4)}");
                    var zero = oneBook.Map(value =>
                    {
                        Console.WriteLine("Map for book: " + value.Title + ", return 0");
                        return Result.Success(0);
                    })
                        .Map(r => r.Value);
                    Console.WriteLine($"Map: to Result<T> to int: {zero}");

                    // Bind: 跟 Maybe 的差異在於 Maybe<T> A => Maybe<T> B，回傳的結果不會改變型態
                    var bookPriceRaise = oneBook.Bind(book =>
                    {
                        book.Price.Add(10);
                        Console.WriteLine($"Bind Raise book {book.Title} price to: {book.Price.Price}");
                        return Maybe.From(book);
                    });

                    // Match: 提供處理有 & 沒有值的方案：
                    var testResult = oneBook.Match(
                        book => $"Match Found book: {book.Title}",
                        () => "Match Not found"
                        );
                }
                else
                {
                    if (Maybe<int>.None == default)
                        Console.WriteLine($"Maybe<int>.None == default ({default})"); // Wrong

                    if (Maybe<int>.None == 0)
                        Console.WriteLine($"Maybe<int>.None == 0"); // Wrong
                    else
                        Console.WriteLine($"Maybe<int>.None is {Maybe<int>.None}"); // Correct! result is "No Value"

                    if (Maybe<string>.None == string.Empty)
                        Console.WriteLine($"Maybe<string>.None == string.Empty");   // Wrong
                    else
                        Console.WriteLine($"Maybe<string>.None is {Maybe<string>.None}");   // Correct

                    if (Maybe<string>.None == null)
                        Console.WriteLine($"Maybe<string>.None == null {Maybe<string>.None}");    // Correct

                    if (Maybe<string>.None == default)
                        Console.WriteLine($"Maybe<string>.None == default");    // Correct

                    if (oneBook == noBook)
                        Console.WriteLine($"no found, equal to Maybe<Book>.None");  // Correct

                    Console.WriteLine("找不到指定的書籍: " + oneBook.GetValueOrDefault(Book.GetDefault("Id 3 is Not Found")).Title);
                }
            }

            Result<Book> aBook = await bookService.GetAsync(iId).ToResult($"Id {iId} Not Found");
            aBook.TapError(message => Console.WriteLine($"async Get error: {message}"));

            break;
        case 4:
            var errorMessage = "Result is Error";
            var noBookResult = bookService.Get(0).ToResult(errorMessage);
            // ToString: 顯示 Result 結果
            Console.WriteLine(noBookResult.ToString());
            noBookResult
                .Map(book =>
                {   // 只會處理成功的情況.
                    Console.WriteLine($"Map: to Book: {book.Title}");
                    return book;
                })
                .MapError(s =>
                {
                    // 只會處理失敗的狀況
                    Console.WriteLine("MapError: " + s);
                    return s;
                });

            var bookResult = Result.Success(bookService.Get(2).Value);
            Console.WriteLine(bookResult.ToString());
            var result = bookResult.Map(book =>  // From Result<T> map to Result<V>
            {
                Console.WriteLine(book.ToString());
                return new { Title = book.Title };
            });

            // Bind 跟 Map 差異：Bind 需要特別指定回傳的 Result type， Map 後直接轉換成 Result type
            var bindResult = noBookResult.Bind(book =>
            {
                //return book;
                return book == null ? Result.Failure("book not found")
                    : Result.Success(new { Title = book.Title });
            });
            bindResult = bookResult.Bind(book => Result.Success(new { Title = book.Title }));

            // Tap: 傳入資料後，再回傳出去，不影響資料的內容
            bookResult
                .Tap(book => Console.WriteLine("Tap: " + book.Title))
                .Tap(() => Console.WriteLine("Tap: logging"))
                .Tap(book =>
                {
                    // Tap 如果變更物件的 property，一樣會改變
                    book.Title = book.Title + " Had been Tapped";
                    Console.WriteLine("Tap Change Title: " + book.Title);
                });

            Result<string> sIdResult = string.Empty;
            Result<string> sPriceResult = string.Empty;
            bool isSuccess = false;
            while (!isSuccess)
            {
                // Implicit Conversion
                Console.Write(" 輸入 Id：");
                sIdResult = Console.ReadLine();
                Console.Write(" 輸入價格：");
                sPriceResult = Console.ReadLine();

                if (Result.Combine(sIdResult, sPriceResult).IsSuccess)
                {
                    isSuccess = int.TryParse(sIdResult.Value, out iId);
                    isSuccess = decimal.TryParse(sPriceResult.Value, out dPrice);
                }
            }

            noBookResult
                .TapError(message => Console.WriteLine($"TapError {message}"))
                .Ensure(book => book == null, "book must be null")  // Ensure 會確保一定要 Success 才會進入判斷，因此以下不會執行
                .Bind(book => Result.Success(Book.GetDefault()))
                .Tap(book => Console.WriteLine($"Not Entr! Default Book: {book.Title}"))
                .Check(book => Result.SuccessIf(book.Price.Price > 0, string.Empty))
                .Tap(book => Console.WriteLine($"Default Book Price Start date: {book.Price.StartDate}"))
                ;

            bookResult
                .Ensure(book => book.Price.Price > 0, "book price must greater than 0")
                .Tap(book => Console.WriteLine($"Get Book: {book.Title}"))
                .Check(book => Result.SuccessIf(book.Price.Price > 0, errorMessage))    // 不能使用 string.Empty
                .Tap(book => Console.WriteLine($"Default Book Price Start date: {book.Price.StartDate}"))
                ;

            errorMessage = bookResult.EnsureNotNull(errorMessage)
                .MapError(message =>
                {
                    Console.WriteLine($"{message}");
                    return message;
                })
                .Map(book => book)
                .Bind(book => Result.Success(book == null ? Book.GetDefault() : book))
                .BindWithTransactionScope(book => Result.Success(book))    // Provide Book Type inner
                    .Check(CheckIfIdIs2)
                    .TapError(message => Console.WriteLine($"TapError is {message}"))
                    .Check(book => AddPriceTest(book.Price.Price, dPrice))
                .Tap(book => Console.WriteLine($"Book Add price to {book.Price.Price}"))
                .Finally(result => result.IsSuccess ? "OK" : result.Error);

            bookResult = bookService.Get(iId).ToResult(errorMessage);
            // Use Ensure as If clause
            bookResult
                .Ensure(book => book.Price.StartDate < DateOnly.FromDateTime(DateTime.Now), $"Ensure Start Date must >= Today")
                .Ensure(book => book.Price.Price > 0, "Price must greater than 0")
                .Ensure(book => !string.IsNullOrWhiteSpace(book.Title), "Book's Title must have value")
                .BindTry(book => BookAddPriceResult(book, dPrice), ErrorHandling)   // Only If inner function throw exception
                .BindIf(book => book.Price.Price > 1000, book =>
                {
                    Console.WriteLine($"BindIf: Add Price: {book.Price.Price}");
                    return BookAddPriceResult(book, dPrice);
                })
                .TapTry(bookService.Update, ErrorHandling)
                .Tap(book => Console.WriteLine($"This book: {book.Title} have passed tests"))
                .TapError(message => Console.WriteLine($"Ensure TapError: {message}"));


            Console.WriteLine($"Finally: {errorMessage}");


            break;
        case 5:
            var date = DateTime.Now;
            var price1 = new BookPrice(10) { StartDate = DateOnly.FromDateTime(date) };
            var price2 = new BookPrice(10) { StartDate = DateOnly.FromDateTime(date) };
            if (price1 == price2) Console.WriteLine("Value Object is equal");
            price2.EndDate = DateOnly.FromDateTime(date);
            if (price1 == price2) Console.WriteLine("Value Object is equal, even is EndDate is not the same");
            price2.Add(10);
            if (price1 == price2) Console.WriteLine("Value Object is equal, even is Price is not the same (WRONG)");


            break;
        case 6:
            bookResult = bookService.Get(2).ToResult(string.Empty);
            bookResult
                .Tap(book => 
                { 
                    book.CoAuthors.Clear(); 
                    book.Comments.Clear();
                    book.Aurthor = new Aurthor { Name = "Bruce Lee", Email = "bruce@example.com" };
                })
                .TapTry(bookService.Update, ErrorHandling);

            bookResult
                .Map(book => {
                    if (book.Comments == null) book.Comments = new();
                    if (book.CoAuthors == null) book.CoAuthors = new();
                    return book;
                })
                .Tap(book => book.Comments.Add("First Comment"))
                .Tap(book => book.Comments.Add("Second Comment"))
                .TapTry(bookService.Update, ErrorHandling)
                .Tap(book => book.CoAuthors.Add(new Aurthor { Name = "Bruce Lee", Email = "bruce@example.com" }))
                .Tap(book => book.CoAuthors.Add(new Aurthor { Name = "Bruce Lee", Email = "bruce1@example.com" }))              
                .TapTry(bookService.Update, ErrorHandling)
                .Check(book => Result.SuccessIf(book.Aurthor == book.CoAuthors[0], "Author is NOT same as CoAuthors[0]"))
                .Check(book => Result.SuccessIf(book.CoAuthors[0]== book.CoAuthors[1], "CoAuthor[0] is NOT same as CoAuthors[1]"))
                .TapError(message => Console.WriteLine(message))
                ;

            Console.WriteLine(bookResult);
            Console.WriteLine(JsonSerializer.Serialize( bookResult.Value));
            break;
        case 0: exit = true; break;
    }
}

static string ErrorHandling(Exception exception)
{
    var message = string.Empty;
    if (exception is DbUpdateException)
    {
        message = $"update exception: {exception.Message}, innert exception: {exception.InnerException.Message}";
    }
    else
    {
        message = exception.Message;
    }
    Console.WriteLine("ErrorHandling:" + exception.ToString());
    return exception.ToString();
}

static Result<Book> BookAddPriceResult(Book book, decimal price)
{
    //var result = AddPriceTest(book.Price.Price, price)
    //    .Bind(p =>
    //    {
    //        book.Price.Set(p);
    //        return Result.Success(book);
    //    });

    var result = AddPriceTest(book.Price.Price, price)
        .Map(p =>
        {
            book.Price.Set(p);
            return book;
        });

    if (result.IsFailure) throw new ArgumentException($"BookAddPriceResult's error {result.Error}");

    return result;
}


static Result<decimal> AddPriceTest(decimal price, decimal number)
{
    var plus = price + number;
    if (plus > 10)
        return Result.Success(plus);
    else
        return Result.Failure<decimal>("Failed: Price less than 100");
}

static Result CheckIfIdIs2(Book book)
{
    return Result.SuccessIf(book.Id == 2, "Book Id is not 2");
}