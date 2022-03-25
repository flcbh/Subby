using Autofac;
using Subby.Utilities.Interfaces;
using MediatR;
using MediatR.Pipeline;
using System.Collections.Generic;
using System.Reflection;
using LastContent.Utilities.Caching;
using LastContent.Utilities.GeoCoordinate;
using LastContent.Utilities.Notification;
using Microsoft.Extensions.Logging;
using Subby.Core.Entities;
using Subby.Utilities.Data;
using Subby.Utilities.DomainEvents;
using Module = Autofac.Module;

namespace Subby.Infrastructure
{
    public class DefaultInfrastructureModule : Module
    {
        private bool _isDevelopment = false;
        private List<Assembly> _assemblies = new List<Assembly>();

        public DefaultInfrastructureModule(bool isDevelopment, Assembly callingAssembly =  null)
        {
            _isDevelopment = isDevelopment;
            var coreAssembly = Assembly.GetAssembly(typeof(Session));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(DomainEvents));
            var utilitiesAssembly = Assembly.GetAssembly(typeof(IRepository));
            _assemblies.Add(coreAssembly);
            _assemblies.Add(infrastructureAssembly);
            _assemblies.Add(utilitiesAssembly);

            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(builder);
            }
            else
            {
                RegisterProductionOnlyDependencies(builder);
            }
            RegisterCommonDependencies(builder);
        }

        private void RegisterCommonDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<RepositoryBase>().As<IRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();
    

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            
            builder.Register(c => new GoogleGeocoder(c.Resolve<ICache>(), c.Resolve<ILogger<GoogleGeocoder>>(), "AIzaSyA2LbhHIYOz1EWe_ld3uBLhFMib6KkUtpk"))
                .As<IGeocoder>()
                .Named<IGeocoder>("Google")
                .InstancePerDependency();

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>),
                typeof(IHandle<>)
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                .RegisterAssemblyTypes(_assemblies.ToArray())
                .AsClosedTypesOf(mediatrOpenType)
                .AsImplementedInterfaces();
            }

            builder.RegisterModule<DomainEventsModule>();
        }

        private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add development only services
        }

        private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add production only services
        }

    }
}
