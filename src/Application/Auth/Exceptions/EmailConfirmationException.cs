namespace Application.Auth.Exceptions;
public sealed class EmailConfirmationException : ApplicationException
{
    public EmailConfirmationException() : base("User is not verified, confirm email") { }
}
