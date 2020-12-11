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

using CMS.Helpers;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Membership;
using Kentico.Web.Mvc;

using XperienceAdapter.Localization;
using Core.Configuration;
using Identity;
using Identity.Models;
using MedioClinic.Configuration;
using MedioClinic.Extensions;
using MedioClinic.Models;
using MedioClinic.Areas.Identity.ModelBinders;
using CMS.Core;
using CMS.DataEngine;
using CMS.SiteProvider;

namespace MedioClinic
{
    public class Startup
    {
        private const string AuthCookieName = "MedioClinic.Authentication";

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
            Options = configuration.GetSection(nameof(XperienceOptions));
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        private IConfigurationSection? Options { get; }

        private string? DefaultCulture => SettingsKeyInfoProvider.GetValue($"{Options?.GetSection("SiteCodeName")}.CMSDefaultCultureCode");

        public AutoFacConfig AutoFacConfig => new AutoFacConfig();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();

            services.AddLocalization();

            services.AddControllersWithViews()
                .AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new UserModelBinderProvider()))
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName!);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });

            services.AddKentico(features =>
            {
                features.UsePageRouting(new PageRoutingOptions { CultureCodeRouteValuesKey = "culture" });
            });

            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = true);

            services.Configure<XperienceOptions>(Options);
            var xperienceOptions = Options.Get<XperienceOptions>();

            ConfigureIdentityServices(services, xperienceOptions);
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
            IOptions<XperienceOptions> optionsAccessor)
        {
            if (WebHostEnvironment.IsDevelopment())
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
                    defaults: new { controller = "Error", action = "Index" }
                    );

                endpoints.MapAreaControllerRoute(
                    name: "identity",
                    areaName: "Identity",
                    pattern: "{culture}/identity/{controller}/{action}/{id?}");

                MapCultureSpecificRoutes(endpoints, optionsAccessor);
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void RegisterInitializationHandler(ContainerBuilder builder) =>
            CMS.Base.ApplicationEvents.Initialized.Execute += (sender, eventArgs) => AutoFacConfig.ConfigureContainer(builder);

        private void MapCultureSpecificRoutes(IEndpointRouteBuilder builder, IOptions<XperienceOptions> optionsAccessor)
        {
            if (AppCore.Initialized)
            {
                var currentSiteName = optionsAccessor.Value.SiteCodeName;
                string? spanishCulture = default;

                if (!string.IsNullOrEmpty(currentSiteName))
                {
                    var cultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName);
                    spanishCulture = cultures.FirstOrDefault(culture => culture.CultureCode.Equals("es-ES")).CultureCode; 
                }

                if (!string.IsNullOrEmpty(DefaultCulture) && !string.IsNullOrEmpty(spanishCulture))
                {
                    var routeOptions = new List<RouteBuilderOptions>
                    {
                        new RouteBuilderOptions("home", new { controller = "Home", action = "Index" }, new[]
                        {
                            (DefaultCulture, "home"),
                            (spanishCulture, "inicio"),
                        }),

                        new RouteBuilderOptions("doctor-listing", new { controller = "Doctors", action = "Index" }, new[]
                        {
                            (DefaultCulture, "doctors"),
                            (spanishCulture, "medicos"),
                        }),

                        new RouteBuilderOptions("doctor-detail", new { controller = "Doctors", action = "Detail" }, new[]
                        {
                            (DefaultCulture, "doctors/{urlSlug?}"),
                            (spanishCulture, "medicos/{urlSlug?}"),
                        }),

                        new RouteBuilderOptions("contact", new { controller = "Contact", action = "Index" }, new[]
                        {
                            (DefaultCulture, "contact-us"),
                            (spanishCulture, "contactenos"),
                        }),
                    };

                    foreach (var options in routeOptions)
                    {
                        foreach (var culture in options.CulturePatterns)
                        {
                            mapRouteCultureVariantsImplementation(culture?.Culture!, options?.RouteName!, culture?.RoutePattern!, options?.RouteDefaults!);
                        }
                    }

                    void mapRouteCultureVariantsImplementation(
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
                            constraints: new { culture = new Kentico.Web.Mvc.Internal.SiteCultureConstraint() }
                            );
                        }
                    }
                }
            }
        }

        private static string AddCulturePrefix(string culture, string pattern) =>
            $"{{culture={culture.ToLowerInvariant()}}}/{pattern}";

        private void ConfigureIdentityServices(IServiceCollection services, XperienceOptions? xperienceOptions)
        {
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPasswordHasher<MedioClinicUser>, Kentico.Membership.PasswordHasher<MedioClinicUser>>();

            services.AddApplicationIdentity<MedioClinicUser, ApplicationRole>()
                .AddApplicationDefaultTokenProviders()
                .AddUserStore<ApplicationUserStore<MedioClinicUser>>()
                .AddRoleStore<ApplicationRoleStore<ApplicationRole>>()
                .AddUserManager<MedioClinicUserManager>()
                .AddSignInManager<MedioClinicSignInManager>();

            var authenticationBuilder = services.AddAuthentication();
            ConfigureExternalAuthentication(services, authenticationBuilder, xperienceOptions);

            services.AddAuthorization();

            services.ConfigureApplicationCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = new PathString("/Account/Signin");

                cookieOptions.Events.OnRedirectToLogin = redirectContext =>
                {
                    var culture = (string)redirectContext.Request.RouteValues["culture"];

                    if (string.IsNullOrEmpty(culture))
                    {
                        culture = DefaultCulture;
                    }

                    var redirectUrl = redirectContext.RedirectUri.Replace("/Account/Signin", $"/{culture}/Account/Signin");
                    redirectContext.Response.Redirect(redirectUrl);
                    return Task.CompletedTask;
                };

                cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(14);
                cookieOptions.SlidingExpiration = true;
                cookieOptions.Cookie.Name = AuthCookieName;
            });

            CookieHelper.RegisterCookie(AuthCookieName, CookieLevel.Essential);
        }

        private static void ConfigureExternalAuthentication(IServiceCollection services, AuthenticationBuilder builder, XperienceOptions? xperienceOptions)
        {
            var identityOptions = xperienceOptions?.IdentityOptions;

            if (identityOptions?.FacebookOptions?.UseFacebookAuth == true)
            {
                var facebookOptions = identityOptions.FacebookOptions;

                builder.AddFacebook(options =>
                {
                    options.ClientId = facebookOptions.AppId;
                    options.ClientSecret = facebookOptions.AppSecret;
                });
            };

            if (identityOptions?.GoogleOptions?.UseGoogleAuth == true)
            {
                var googleOptions = identityOptions.GoogleOptions;

                builder.AddGoogle(options =>
                {
                    options.ClientId = googleOptions.ClientId;
                    options.ClientSecret = googleOptions.ClientSecret;
                });
            };

            if (identityOptions?.MicrosoftOptions?.UseMicrosoftAuth == true)
            {
                var microsoftOptions = identityOptions.MicrosoftOptions;

                builder.AddMicrosoftAccount(options =>
                {
                    options.ClientId = microsoftOptions.ClientId;
                    options.ClientSecret = microsoftOptions.ClientSecret;
                });
            };

            if (identityOptions?.TwitterOptions?.UseTwitterAuth == true)
            {
                var twitterOptions = identityOptions.TwitterOptions;

                builder.AddTwitter(options =>
                {
                    options.ConsumerKey = twitterOptions.ConsumerKey;
                    options.ConsumerSecret = twitterOptions.ConsumerSecret;
                    options.RetrieveUserDetails = true;
                });
            }
        }
    }
}
