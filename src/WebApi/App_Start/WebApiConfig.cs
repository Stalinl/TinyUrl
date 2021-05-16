namespace TinyUrl.WebApi
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    using Autofac;
    using Autofac.Integration.WebApi;
    using EnsureThat;
    using TinyUrl.Core;

    /// <summary>
    /// Class encapsulating API Controller Registration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers API Controllers and dependencies.
        /// </summary>
        /// <param name="config">The configuration to inject.</param>
        public static void Register(HttpConfiguration config)
        {
            var conString = GetConfig("DB.ConnectionStringKey");
            var tinyUrlBaseAddress = GetConfig("TinyUrlBaseAddress");

            var builder = new ContainerBuilder();
            builder
                .AddAssemblyApis()
                .AddRepository(conString)
                .AddTinyUrlService(tinyUrlBaseAddress)
                .RegisterWebApiFilterProvider(config);

            config
                .AddRouting()
                .AddDependencyResolver(builder)
                .AddExceptionHandler();
        }

        internal static ContainerBuilder AddRepository(this ContainerBuilder builder, string connectionString)
        {
            EnsureArg.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));

            builder.Register(
                context =>
                {
                    return new SqlRepository(connectionString);
                })
                .As<IRepository>()
                .SingleInstance();

            return builder;
        }

        internal static ContainerBuilder AddTinyUrlService(this ContainerBuilder builder, string tinyUrlBaseAddress)
        {
            EnsureArg.IsNotNullOrWhiteSpace(tinyUrlBaseAddress, nameof(tinyUrlBaseAddress));

            builder.Register(
                context =>
                {
                    return new TinyUrlService(context.Resolve<IRepository>(), new Uri(tinyUrlBaseAddress));
                })
                .As<ITinyUrlService>()
                .SingleInstance();

            return builder;
        }

        internal static ContainerBuilder AddAssemblyApis(this ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            return builder;
        }

        internal static HttpConfiguration AddDependencyResolver(this HttpConfiguration config, ContainerBuilder builder)
        {
            EnsureArg.IsNotNull(builder, nameof(builder));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
            return config;
        }

        internal static HttpConfiguration AddRouting(this HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            return config;
        }

        private static HttpConfiguration AddExceptionHandler(this HttpConfiguration config)
        {
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            return config;
        }

        private static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}