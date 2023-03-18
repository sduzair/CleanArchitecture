namespace Application.Authentication.Exceptions;

// create application exception class for user exists exception in application layer 
public sealed class UserExistsException : ApplicationException
{
    public UserExistsException() : base("User already exists") { }
}
