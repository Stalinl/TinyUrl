namespace TinyUrl.WebApi
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http;

    public class Global : System.Web.HttpApplication
    {
        [ExcludeFromCodeCoverage]
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}