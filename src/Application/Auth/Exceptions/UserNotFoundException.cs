namespace Application.Auth.Exceptions;

public sealed class UserNotFoundException : ApplicationException
{
    public UserNotFoundException() : base("User not found") { }
}

