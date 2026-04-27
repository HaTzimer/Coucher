namespace Coucher.Shared.Exceptions;

public sealed class CoucherAuthorizationException : Exception
{
    public CoucherAuthorizationException(string message)
        : base(message)
    {
    }
}
