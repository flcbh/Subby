using System.Linq;
using System.Reflection;
using Autofac;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;
using Module = Autofac.Module;

namespace Subby.Infrastructure
{
    internal class DomainEventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            var asm = Assembly.GetExecutingAssembly();
            var handlerType = typeof(IHandle<>);

            builder.RegisterAssemblyTypes(asm)
                .Where(t => t.GetInterfaces().Any(t => t.IsClosedTypeOf(handlerType)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope(); 
            
            builder.Register(x =>
            {
                var cc = x.Resolve<IComponentContext>();
                return new DomainEvents(type =>
                {
                    var test = cc.Resolve(type);
                    return test;
                });
            }).As<IDomainEvents>();
        }
    }

}