using Domain.Objects;

namespace Domain.Entities;

public class User : IAggregateRoot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Email { get; init; }
    public string Password { get; init; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? ZipCode { get; private set; }
    public string? Country { get; private set; }
    private User(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public static User Create(string email, string password, string confirmPassword)
    {
        CheckPasswordMatch(password, confirmPassword);
        return new User(email, password);
    }

    private static void CheckPasswordMatch(string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            throw new Exception("Passwords do not match");
        }
    }

    public void Update(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
