using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coacher.WebApi.Filters;

public sealed class CoacherHttpStatusCodeExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not HttpStatusCodeException exception)
            return;

        context.Result = new ObjectResult(exception.ToJObject())
        {
            StatusCode = (int)exception.StatusCode
        };
        context.ExceptionHandled = true;
    }
}
