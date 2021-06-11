namespace net.atos.daf.ct2.exceptionhandling
{
    public static class ExceptionMiddlewareExtensions
    {
        //public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger,bool isDevelopement)
        // {
        //    app.UseExceptionHandler(appError =>
        //    {
        //        appError.Run(async context =>
        //        {
        //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //            context.Response.ContentType = "application/json";
        //            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        //            if(contextFeature != null)
        //            { 
        //                // Logging exception
        //                logger.LogError($"Something went wrong: {contextFeature.Error}");
        //                //TODO: Need to identify more exceptions type and have check.

        //                HttpStatusCode status = HttpStatusCode.InternalServerError;
        //                String message = String.Empty;

        //                var exceptionType = contextFeature.Error.GetType();

        //                if (exceptionType == typeof(BadRequestResult))
        //                {
        //                    message = "Bad Request Result.";
        //                    status = HttpStatusCode.BadRequest;
        //                }                          
        //                else if (exceptionType == typeof(UnauthorizedAccessException))
        //                {
        //                    message = "Unauthorized Access";
        //                    status = HttpStatusCode.Unauthorized;
        //                }
        //                else if (exceptionType == typeof(NotFoundResult))
        //                {
        //                    message = contextFeature.Error.ToString();
        //                    status = HttpStatusCode.NotFound;
        //                }

        //                else if (exceptionType == typeof(NotImplementedException))
        //                {
        //                    message = "Internal server error.";
        //                    status = HttpStatusCode.NotImplemented;
        //                }                                                                 
        //                else
        //                {
        //                     message = "Internal server error.";
        //                    status = HttpStatusCode.NotFound;
        //                }

        //                // ExceptionDetails exceptionDetails = new ExceptionDetails();
        //                // exceptionDetails.StatusCode=status;                        
        //                // if(isDevelopement)
        //                // {
        //                //     exceptionDetails.Message = "Error Occured - Message - " + contextFeature.Error.Message + "Stack Trace:" +  contextFeature.Error.StackTrace;
        //                // }
        //                // exceptionDetails.Message = message;
        //                //await context.Response.WriteAsync(exceptionDetails.ToString());                        
        //            }
        //        });
        //    });
        //}
    }
}

