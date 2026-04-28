using Coucher.Shared.Exceptions;
using HotChocolate;

namespace Coucher.WebApi.GraphQl;

public sealed class CoucherGraphQlErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is not CoucherAuthorizationException authorizationException)
            return error;

        var filteredError = error
            .WithMessage(authorizationException.Message)
            .WithCode("AUTHORIZATION_FORBIDDEN");

        return filteredError;
    }
}
