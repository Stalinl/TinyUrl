namespace TinyUrl.WebApi
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Results;

    using TinyUrl.Core;

    internal sealed class GlobalExceptionHandler : ExceptionHandler
    {
        [ExcludeFromCodeCoverage]
        public override void Handle(ExceptionHandlerContext context)
        {
            // log the error. context.Exception
            var response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError, Resources.UnhandledError);

            context.Result = new ResponseMessageResult(response);
        }
    }
}