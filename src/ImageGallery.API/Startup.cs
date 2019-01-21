using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using ImageGallery.API.Authorization;
using ImageGallery.API.Entities;
using ImageGallery.API.Services;
using ImageGallery.API.Settings;
using ImageGallery.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Image = ImageGallery.API.Entities.Image;

[assembly: ApiConventionType(typeof(ImageGallery.API.Conventions.DefaultApiConventions))]

namespace ImageGallery.API
{
    public class Startup
    {
        public Startup(IOptions<TokenProviderSettings> tokenProviderSettings, IOptions<DbSettings> dbSettings)
        {
            TokenProviderSettings = tokenProviderSettings.Value;
            DbSettings = dbSettings.Value;
        }

        private TokenProviderSettings TokenProviderSettings { get; }
        private DbSettings DbSettings { get; }

        /// <summary>
        ///     Add services to the DI container.
        /// </summary>
        /// <remarks>
        ///     This method gets called by the runtime
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "MustOwnImage",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(
                            new MustOwnImageRequirement());
                    });
            });

            services.AddScoped<IAuthorizationHandler, MustOwnImageHandler>();

            services.AddAuthentication(
                    IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = TokenProviderSettings.BaseUrl;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "imagegalleryapi";
                    options.ApiSecret = "apisecret";
                });

            services.AddDbContext<GalleryContext>(o => o.UseSqlServer(DbSettings.ConnectionString));

            // register the repository
            services.AddScoped<IGalleryRepository, GalleryRepository>();

            services.AddSwaggerDocument(
                configure =>
                {
                    configure.PostProcess = (document) =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "ImageGalleryAPI";
                        document.Info.Description = "Image Gallery API";
                    };
                });
        }

        /// <summary>
        ///     Configures the HTTP request pipeline
        /// </summary>
        /// <remarks>
        ///     This method gets called by the runtime
        /// </remarks>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, GalleryContext galleryContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        // ensure generic 500 status code on fault.
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            app.UseHealthChecks("/api/health");

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseAuthentication();

            app.UseStaticFiles();

            Mapper.Initialize(cfg =>
            {
                // Map from Image (entity) to Image, and back
                cfg.CreateMap<Image, Model.Image>().ReverseMap();

                // Map from ImageForCreation to Image
                // Ignore properties that shouldn't be mapped
                cfg.CreateMap<ImageForCreation, Image>()
                    .ForMember(m => m.FileName, options => options.Ignore())
                    .ForMember(m => m.Id, options => options.Ignore())
                    .ForMember(m => m.OwnerId, options => options.Ignore());

                // Map from ImageForUpdate to Image
                // ignore properties that shouldn't be mapped
                cfg.CreateMap<ImageForUpdate, Image>()
                    .ForMember(m => m.FileName, options => options.Ignore())
                    .ForMember(m => m.Id, options => options.Ignore())
                    .ForMember(m => m.OwnerId, options => options.Ignore());
            });

            Mapper.AssertConfigurationIsValid();

            app.UseMvc();
        }

        /// <summary>
        ///     Configure services that are going to be injected into <see cref="Startup" />
        /// </summary>
        public static void ConfigureStartupServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            services.Configure<TokenProviderSettings>(ctx.Configuration.GetSection("TokenProvider"));
            services.Configure<DbSettings>(ctx.Configuration.GetSection("Db"));
        }
    }
}