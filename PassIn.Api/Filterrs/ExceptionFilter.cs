using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using System.Net;

namespace PassIn.Api.Filterrs;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var result = context.Exception is PassInException;
        if (result)
        {
            HandleProjectException(context);
        }
        else
        {
            ThrowUnknownError(context);
        }
        
    }

    private void HandleProjectException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        context.Result = new ObjectResult(new ResponseErrorJson(context.Exception.Message));
    }

    private void ThrowUnknownError(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        context.Result = new ObjectResult(new ResponseErrorJson("Unknown error"));
    }
}
