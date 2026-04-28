using Augustus.Infra.Core.Shared.Exceptions;
using System.Net;
using HotChocolate;

namespace Coacher.WebApi.GraphQl;

public sealed class CoacherGraphQlErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is not HttpStatusCodeException exception)
            return error;

        var filteredError = error
            .WithMessage(exception.Message)
            .WithCode(GetErrorCode(exception.StatusCode));

        return filteredError;
    }

    private static string GetErrorCode(HttpStatusCode statusCode)
    {
        var errorCode = statusCode switch
        {
            HttpStatusCode.BadRequest => "BAD_REQUEST",
            HttpStatusCode.Unauthorized => "UNAUTHORIZED",
            HttpStatusCode.Forbidden => "AUTHORIZATION_FORBIDDEN",
            HttpStatusCode.NotFound => "NOT_FOUND",
            HttpStatusCode.Conflict => "CONFLICT",
            _ => "HTTP_STATUS_ERROR"
        };

        return errorCode;
    }
}
