using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Kentico.Membership;
using Kentico.Web.Mvc;
using Kentico.Web.Mvc.Internal;
using Business.Configuration;
using Identity.Models;
using MedioClinic.Configuration;
using MedioClinic.Extensions;
using MedioClinic.Models;
using Identity;
using MedioClinic.Areas.Identity.ModelBinders;

namespace MedioClinic
{
    public class Startup
    {
        private const string DefaultCultureFallback = "en-US";
        private const string AuthCookieName = "MedioClinic.Authentication";

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
            services.AddResponseCaching();

            services.AddControllersWithViews()
                .AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new UserModelBinderProvider()));

            services.AddKentico();
            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = true);

            var xperienceOptions = Configuration.GetSection(nameof(XperienceOptions));
            services.Configure<XperienceOptions>(xperienceOptions);

            ConfigureIdentityServices(services, xperienceOptions);

            // Load external authentication configurations

            var googleAuthenticationOptions = Configuration.GetSection(nameof(GoogleAuthenticationOptions));
            var twitterAuthenticationOptions = Configuration.GetSection(nameof(TwitterAuthenticationOptions));
            var msAuthenticationOptions = Configuration.GetSection(nameof(MicrosoftAuthenticationOptions));
            var facebookAuthenticationOptions = Configuration.GetSection(nameof(FacebookAuthenticationOptions));

            services.Configure<GoogleAuthenticationOptions>(googleAuthenticationOptions);
            services.Configure<MicrosoftAuthenticationOptions>(msAuthenticationOptions);
            services.Configure<FacebookAuthenticationOptions>(facebookAuthenticationOptions);
            services.Configure<TwitterAuthenticationOptions>(twitterAuthenticationOptions);

            ConfigureExternalAuthentication(services, googleAuthenticationOptions, msAuthenticationOptions, facebookAuthenticationOptions, twitterAuthenticationOptions);
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
            app.UseResponseCaching();
            app.UseRequestCulture();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "areas",
                //    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "identity",
                    areaName: "Identity",
                    pattern: "identity/{controller=Account}/{action=Register}/{id?}");

                MapCultureSpecificRoutes(endpoints, optionsAccessor);
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void RegisterInitializationHandler(ContainerBuilder builder) =>
            CMS.Base.ApplicationEvents.Initialized.Execute += (sender, eventArgs) => AutoFacConfig.ConfigureContainer(builder);

        private void MapCultureSpecificRoutes(IEndpointRouteBuilder builder, IOptions<XperienceOptions> optionsAccessor)
        {
            var defaultCulture = optionsAccessor.Value.DefaultCulture ?? DefaultCultureFallback;
            var spanishCulture = "es-ES";

            var routeOptions = new List<RouteBuilderOptions>
            {
                new RouteBuilderOptions("home", new { controller = "Home", action = "Index" }, new[]
                {
                    (defaultCulture, "home"),
                    (spanishCulture, "inicio"),
                }),

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
                    constraints: new { culture = new SiteCultureConstraint() }
                    );
                }
            }
        }

        // TODO: Can the segment name be inferred?
        private static string AddCulturePrefix(string culture, string pattern) =>
            $"{{culture={culture}}}/{pattern}";

        private static void ConfigureExternalAuthentication(
            IServiceCollection services,
            IConfigurationSection googleAuthenticationOptions,
            IConfigurationSection msAuthenticationOptions,
            IConfigurationSection facebookAuthenticationOptions,
            IConfigurationSection twitterAuthenticationOptions)
        {
            var authBuilder = services.AddAuthentication();

            var useGoogleAuth = googleAuthenticationOptions.Get<GoogleAuthenticationOptions>().UseGoogleAuth;
            if (useGoogleAuth)
            {
                authBuilder.AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = googleAuthenticationOptions.Get<GoogleAuthenticationOptions>()?.ClientId;
                    googleOptions.ClientSecret = googleAuthenticationOptions.Get<GoogleAuthenticationOptions>()?.ClientSecret;
                });
            }

            var useMSAuth = msAuthenticationOptions.Get<MicrosoftAuthenticationOptions>().UseMicrosoftAuth;
            if (useMSAuth)
            {
                authBuilder.AddMicrosoftAccount(microsoftAccountOptions =>
                {
                    microsoftAccountOptions.ClientSecret = msAuthenticationOptions.Get<MicrosoftAuthenticationOptions>()?.ClientSecret;
                    microsoftAccountOptions.ClientId = msAuthenticationOptions.Get<MicrosoftAuthenticationOptions>()?.ClientId;
                });
            }

            var useFacebookAuth = facebookAuthenticationOptions.Get<FacebookAuthenticationOptions>().UseFacebookAuth;
            if(useFacebookAuth)
            {
                authBuilder.AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = facebookAuthenticationOptions.Get<FacebookAuthenticationOptions>()?.AppId;
                    facebookOptions.AppSecret = facebookAuthenticationOptions.Get<FacebookAuthenticationOptions>()?.AppSecret;
                });
            }

            var useTwitterAuth = twitterAuthenticationOptions.Get<TwitterAuthenticationOptions>().UseTwitterAuth;
            if (useTwitterAuth)
            {
                authBuilder.AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = twitterAuthenticationOptions.Get<TwitterAuthenticationOptions>()?.ConsumerKey;
                    twitterOptions.ConsumerSecret = twitterAuthenticationOptions.Get<TwitterAuthenticationOptions>()?.ConsumerSecret;
                    twitterOptions.RetrieveUserDetails = true;
                });
            }
        }

        private static void ConfigureIdentityServices(IServiceCollection services, IConfigurationSection xperienceOptions)
        {
            services.AddScoped<IPasswordHasher<MedioClinicUser>, Kentico.Membership.PasswordHasher<MedioClinicUser>>();
            services.AddScoped<IMessageService, MessageService>();

            services.AddApplicationIdentity<MedioClinicUser, ApplicationRole>()
                .AddUserStore<ApplicationUserStore<MedioClinicUser>>()
                .AddRoleStore<ApplicationRoleStore<ApplicationRole>>()
                .AddUserManager<MedioClinicUserManager>()
                .AddSignInManager<MedioClinicSignInManager>();

            //services.AddHttpContextAccessor();
            //services.AddScoped(typeof(ISecurityStampValidator), typeof(SecurityStampValidator<>).MakeGenericType(typeof(MedioClinicUser)));
            //services.AddScoped(typeof(ITwoFactorSecurityStampValidator), typeof(TwoFactorSecurityStampValidator<>).MakeGenericType(typeof(MedioClinicUser)));
            //services.AddScoped<IMedioClinicSignInManager<MedioClinicUser>, MedioClinicSignInManager>();

            services.AddAuthorization();
            
            services.ConfigureApplicationCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = new PathString("/Account/Signin");

                cookieOptions.Events.OnRedirectToLogin = redirectContext =>
                {
                    var culture = (string)redirectContext.Request.RouteValues["culture"];

                    if (string.IsNullOrEmpty(culture))
                    {
                        culture = xperienceOptions.Get<XperienceOptions>()?.DefaultCulture ?? DefaultCultureFallback;
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
    }
}
