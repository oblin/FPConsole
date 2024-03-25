using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace FpConsole;

public class Aurthor : ValueObject
{
    public string Name { get; set; }

    public string Email { get; set; }
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Name;
        yield return Email;
    }
}

/// <summary>
/// Nested Complex Type is not working on EF Core8, Plus required issue
/// It won't be a good choice right now.
/// </summary>
public class Email : ValueObject
{
    public string Value { get; }
    
    public Email() { }
    public Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(Maybe<string> emailOrNothing)
    {
        return emailOrNothing.ToResult("Email should not be empty")
            .Map(email => email.Trim())
            .Ensure(email => email != string.Empty, "Email should not be empty")
            .Ensure(email => email.Length <= 256, "Email is too long")
            .Ensure(email => Regex.IsMatch(email, @"^(.+)@(.+)$"), "Email is invalid")
            .Map(email => new Email(email));
    }

    public static explicit operator Email(string email)
    {
        return Create(email).Value;
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}