using System.Net;
using System.Text.Json;
using Core.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next,ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"An unhandeled exception occured: {ex.Message}",ex.Message);
                await HandleExceptionAsync(context,ex);
            }
        }

        public static async Task HandleExceptionAsync(HttpContext context,Exception ex)
        {
            context.Response.ContentType="application/json";

            HttpStatusCode statusCode=HttpStatusCode.InternalServerError;
            string title="Internal Server Error";
            switch (ex)
            {
                case NotFoundException:
                    statusCode=HttpStatusCode.NotFound;
                    title="Resource Not Found";
                    break;
                case BadRequestException:
                    statusCode=HttpStatusCode.BadRequest;
                    title="Bad Request";
                    break;
                case ArgumentException:
                    statusCode=HttpStatusCode.BadRequest;
                    title="Invalida Argument";
                    break;
                default:
                    break;
            }
            context.Response.StatusCode=(int)statusCode;
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;
            var problemDetails=new ProblemDetails
            {
                Status=(int)statusCode,
                Title=title,
                Detail=statusCode==HttpStatusCode.InternalServerError
                                    ?"An unexpected internal server error occured"
                                    :ex.Message,
                Instance=context.Request.Path,
                Extensions =
                {
                    ["correlationId"]=correlationId
                }
            };
            var json=JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        } 
    }
}