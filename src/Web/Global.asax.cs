namespace TinyUrl.Web
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.Routing;

    [ExcludeFromCodeCoverage]
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            IoCContainerConfig.Register();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}