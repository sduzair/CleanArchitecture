namespace Application.Auth.Exceptions;
public sealed class IncorrectPasswordException : ApplicationException
{
    public IncorrectPasswordException() : base("Incorrect password") { }
}
