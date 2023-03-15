namespace Application.Authentication;

// create application exception class for user exists exception in application layer 
public class UserExistsException : ApplicationException
{
    public UserExistsException() : base("User already exists") { }
}
