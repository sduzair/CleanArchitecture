namespace Application.Authentication.Exceptions;
public sealed class UserNotVerifiedException : ApplicationException
{
    public UserNotVerifiedException() : base("User is not verified, confirm email") { }
}
