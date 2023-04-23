namespace Application.Auth.Exceptions;

public sealed class UserExistsException : ApplicationException
{
    public UserExistsException() : base("User already exists") { }
}
