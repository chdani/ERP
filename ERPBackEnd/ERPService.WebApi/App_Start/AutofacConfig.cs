using System;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutofacSerilogIntegration;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Middleware;
using ERPService.Common.Shared;
using ERPService.Data;

namespace ERPService.WebApi
{
    /// <summary>
    /// Represent Autofac configuration.
    /// </summary>
    public class AutofacConfig
    {
        protected internal static IContainer Container;

        public static void Configure(HttpConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var builder = new ContainerBuilder();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
            builder.RegisterLogger();
            builder.Register(c => HttpContext.Current.User.Identity).InstancePerRequest();
            builder.RegisterInstance(AutoMapperConfig.Configure(config));
            builder.RegisterType<ContextMiddleware>().InstancePerRequest();
            builder.RegisterType<UserContext>().InstancePerRequest();

            builder.RegisterType<AppDbContext>().InstancePerRequest();
            builder.RegisterType<EfRepository>().As<IRepository>().InstancePerLifetimeScope();

            RegisterServices(builder);
            
            Container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
            
        }

        private static void RegisterServices(ContainerBuilder builder)
        {

            // TODO: Register additional services for injection
            // For more information see https://github.com/drwatson1/AspNet-WebApi/wiki#dependency-injection
            // builder.Register(c => new UserContext()).SingleInstance();
        }
    }
}