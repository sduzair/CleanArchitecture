namespace Application.Authentication.Exceptions;
public sealed class EmailConfirmatioException : ApplicationException
{
    public EmailConfirmatioException() : base("User is not verified, confirm email") { }
}
