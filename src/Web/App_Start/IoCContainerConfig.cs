namespace TinyUrl.Web
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Mvc;

    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using EnsureThat;
    using TinyUrl.Core;

    [ExcludeFromCodeCoverage]
    public static class IoCContainerConfig
    {
        /// <summary>
        /// Registers the dependencies for the application.
        /// </summary>
        public static void Register()
        {
            var conString = GetConfig("DB.ConnectionStringKey");
            var tinyUrlBaseAddress = GetConfig("TinyUrlBaseAddress");
            var builder = new ContainerBuilder();

            builder
                .RegisterDependencies()
                .AddRepository(conString)
                .AddTinyUrlService(tinyUrlBaseAddress)
                .AddDependencyResolver();
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

        private static ContainerBuilder RegisterDependencies(this ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            return builder;
        }

        private static ContainerBuilder AddDependencyResolver(this ContainerBuilder builder)
        {
            var container = builder.Build();
            IDependencyResolver resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);

            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;
            return builder;
        }

        private static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}