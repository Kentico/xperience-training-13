using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Autofac;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Membership;
using Kentico.Web.Mvc;

using Common.Configuration;
using XperienceAdapter.Localization;
using MedioClinic.Configuration;
using MedioClinic.Extensions;
using MedioClinic.Models;

namespace MedioClinic
{
    public class Startup
    {
        private const string ConventionalRoutingControllers = "Error";

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public IConfigurationSection? Options { get; }

        public string? DefaultCulture => SettingsKeyInfoProvider.GetValue($"{Options?.GetSection("SiteCodeName")}.CMSDefaultCultureCode");

        public AutoFacConfig AutoFacConfig => new AutoFacConfig();

        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            Environment = webHostEnvironment;
            Configuration = configuration;
            Options = configuration.GetSection(nameof(XperienceOptions));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Enable desired Kentico Xperience features
            var kenticoServiceCollection = services.AddKentico(features =>
            {
                // features.UsePageBuilder();
                // features.UseActivityTracking();
                // features.UseABTesting();
                // features.UseWebAnalytics();
                // features.UseEmailTracking();
                // features.UseCampaignLogger();
                // features.UseScheduler();
                features.UsePageRouting(new PageRoutingOptions { CultureCodeRouteValuesKey = "culture" });
            });

            if (Environment.IsDevelopment())
            {
                // By default, Xperience sends cookies using SameSite=Lax. If the administration and live site applications
                // are hosted on separate domains, this ensures cookies are set with SameSite=None and Secure. The configuration
                // only applies when communicating with the Xperience administration via preview links. Both applications also need 
                // to use a secure connection (HTTPS) to ensure cookies are not rejected by the client.
                kenticoServiceCollection.SetAdminCookiesSameSiteNone();

                // By default, Xperience requires a secure connection (HTTPS) if administration and live site applications
                // are hosted on separate domains. This configuration simplifies the initial setup of the development
                // or evaluation environment without a the need for secure connection. The system ignores authentication
                // cookies and this information is taken from the URL.
                kenticoServiceCollection.DisableVirtualContextSecurityForLocalhost();
            }

            services.AddAuthentication();
            // services.AddAuthorization();

            services.AddLocalization();
            services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = false);

            services.AddControllersWithViews()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = typeof(SharedResource).GetTypeInfo().Assembly.GetName().Name;

                        return factory.Create("SharedResource", assemblyName);
                    };
                });

            services.Configure<XperienceOptions>(Options);
            var xperienceOptions = Options.Get<XperienceOptions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                        await context.Response.WriteAsync("An error happened.<br><br>\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

                        if (exceptionHandlerPathFeature?.Error is System.IO.FileNotFoundException)
                        {
                            await context.Response.WriteAsync("A file error happened.<br><br>\r\n");
                        }

                        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                    });
                });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseLocalizedStatusCodePagesWithReExecute("/{0}/error/{1}/");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseKentico();

            app.UseRouting();

            app.UseRequestCulture();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Kentico().MapRoutes();

                endpoints.MapControllerRoute(
                    name: "error",
                    pattern: "{culture}/error/{code}",
                    defaults: new { controller = "Error", action = "Index" },
                    constraints: new
                    {
                        controller = ConventionalRoutingControllers
                    });

                endpoints.MapDefaultControllerRoute();
            });
        }

        /// <summary>
        /// Registers a handler in case Xperience is not initialized yet.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        private void RegisterInitializationHandler(ContainerBuilder builder) =>
            CMS.Base.ApplicationEvents.Initialized.Execute += (sender, eventArgs) => AutoFacConfig.ConfigureContainer(builder);

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
    }
}
