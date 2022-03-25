using System;
using System.IO;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Instances;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Subby.Utilities.Data;
using Subby.Utilities.Interfaces;
using Environment = System.Environment;

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