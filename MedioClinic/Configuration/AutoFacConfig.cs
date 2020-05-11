using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;
using Abstractions;

namespace MedioClinic.Configuration
{
    public class AutoFacConfig
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
