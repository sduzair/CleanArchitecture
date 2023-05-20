using System.Runtime.Serialization;

namespace Presentation.Exceptions;

[Serializable]
internal sealed class ClaimsPrincipalNullException : Exception
{
    public ClaimsPrincipalNullException() : base("ClaimsPrincipal is null")
    {
    }

    public ClaimsPrincipalNullException(string? message) : base(message)
    {
    }

    public ClaimsPrincipalNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
