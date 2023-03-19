namespace Application.Authentication.Exceptions;

public sealed class UserExistsException : ApplicationException
{
    public UserExistsException() : base("User already exists") { }
}
