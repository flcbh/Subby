using Microsoft.Extensions.DependencyInjection;
using Subby.Utilities.Data;
using Subby.Utilities.Interfaces;
using System.Reflection;

namespace Subby.Infrastructure.Data
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionName, Assembly entityAssembly, Assembly mappingAssembly)
        {
            var sessionFactory = LastContent.Utilities.Data.NHibernateExtensions.SessionFactory(connectionName, entityAssembly, mappingAssembly, true);
            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddScoped<IRepository, RepositoryBase>();
  
            return services;
        }
    }
}