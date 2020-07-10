using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Kentico.Web.Mvc.Internal;

using Business.Configuration;
using Business.Services;
using MedioClinic.Configuration;
using MedioClinic.Extensions;
using MedioClinic.Models;

namespace MedioClinic
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public AutoFacConfig AutoFacConfig => new AutoFacConfig();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddKentico();
            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = true);
            services.Configure<XperienceOptions>(Configuration.GetSection(nameof(XperienceOptions)));
            var optionsServiceType = WebHostEnvironment.IsDevelopment() ? typeof(DevelopmentOptionsService<>) : typeof(ProductionOptionsService<>);
            services.AddSingleton(typeof(IOptionsService<>), optionsServiceType);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            try
            {
                AutoFacConfig.ConfigureContainer(builder);
            }
            catch
            {
                RegisterInitializationHandler(builder);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IOptionsService<XperienceOptions> optionsService)
        {
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseKentico(features =>
            {
                features.UsePreview();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestCulture();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                MapCultureSpecificRoutes(endpoints, optionsService);
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void RegisterInitializationHandler(ContainerBuilder builder) =>
            CMS.Base.ApplicationEvents.Initialized.Execute += (sender, eventArgs) => AutoFacConfig.ConfigureContainer(builder);

        private static string AddCulturePrefix(string defaultCulture, string pattern) =>
            $"{{culture={defaultCulture}}}/{pattern}";

        private void MapCultureSpecificRoutes(IEndpointRouteBuilder builder, IOptionsService<XperienceOptions> optionsService)
        {
            var defaultCulture = optionsService.Options.DefaultCulture ?? "en-US";
            var spanishCulture = "es-ES";

            var test = new List<RouteBuilderOptions>
            {
                new RouteBuilderOptions("doctor-listing", new { controller = "Doctors", action = "Index" }, new[]
                {
                    (defaultCulture, "doctors"),
                    (spanishCulture, "medicos"),
                }),

                new RouteBuilderOptions("doctor-detail", new { controller = "Doctors", action = "Detail" }, new[]
                {
                    (defaultCulture, "doctors/{nodeGuid}/{urlSlug?}"),
                    (spanishCulture, "medicos/{nodeGuid}/{urlSlug?}"),
                }),

                new RouteBuilderOptions("contact", new { controller = "Contact", action = "Index" }, new[]
                {
                    (defaultCulture, "contact"),
                    (spanishCulture, "contactenos"),
                }),
            };

            foreach (var options in test)
            {
                foreach (var culture in options.CulturePatterns)
                {
                    mapRouteCultureVariantsImplementation(builder, culture?.Culture!, options?.RouteName!, culture?.RoutePattern!, options?.RouteDefaults!);
                }
            }

            void mapRouteCultureVariantsImplementation(
                IEndpointRouteBuilder builder1,
                string culture,
                string routeName,
                string routePattern,
                object routeDefaults)
            {
                var stringParameters = new string[] { culture, routeName, routePattern };

                if (stringParameters.All(parameter => !string.IsNullOrEmpty(parameter)) && routeDefaults != null)
                {
                    builder.MapControllerRoute(
                    name: $"{routeName}_{culture}",
                    pattern: AddCulturePrefix(culture, routePattern!),
                    defaults: routeDefaults,
                    constraints: new { culture = new SiteCultureConstraint() }
                    );
                }
            }
        }
    }
}
