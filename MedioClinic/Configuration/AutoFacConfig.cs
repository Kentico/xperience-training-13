using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;
using Abstractions;
using System.Reflection;
using XperienceAdapter;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Rest.Azure;

namespace MedioClinic.Configuration
{
    public class AutoFacConfig
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IService).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(type => type.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        @interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRepository<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(type => type.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        @interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IPageRepository<,>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();



















            //builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            //    .Where(type =>
            //    {
            //        // TODO: Simplify.
            //        var interfaces = type.GetTypeInfo().ImplementedInterfaces;
            //        var filtered = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPageRepository<,>));
            //        var any = filtered.Any();
            //        return any;
            //    })
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            builder.RegisterType<RepositoryDependencies>().As<IRepositoryDependencies>()
                .InstancePerLifetimeScope();
        }
    }
}
