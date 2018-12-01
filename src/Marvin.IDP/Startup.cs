using Marvin.IDP.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marvin.IDP
{
    public class Startup
    {
        public Startup(IOptions<TokenProviderSettings> tokenProviderSettings,
            IOptions<GalleryOidcClientSettings> galleryOidcClientSettings)
        {
            GalleryOidcClientSettings = galleryOidcClientSettings.Value;
            TokenProviderSettings = tokenProviderSettings.Value;
        }

        private GalleryOidcClientSettings GalleryOidcClientSettings { get; }

        private TokenProviderSettings TokenProviderSettings { get; }

        /// <summary>
        ///     Add services to the DI container.
        /// </summary>
        /// <remarks>
        ///     This method gets called by the runtime
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer(options =>
                {
                    if (!string.IsNullOrEmpty(TokenProviderSettings.BaseUrl))
                    {
                        options.IssuerUri = TokenProviderSettings.BaseUrl;
                        options.PublicOrigin = TokenProviderSettings.BaseUrl;
                    }
                })
                .AddDeveloperSigningCredential()
                .AddTestUsers(Config.GetUsers())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(GalleryOidcClientSettings));
        }


        /// <summary>
        ///     Configures the HTTP request pipeline
        /// </summary>
        /// <remarks>
        ///     This method gets called by the runtime
        /// </remarks>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        /// <summary>
        ///     Configure services that are going to be injected into <see cref="Startup" />
        /// </summary>
        public static void ConfigureStartupServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            services.Configure<TokenProviderSettings>(ctx.Configuration.GetSection("TokenProvider"));
            services.Configure<GalleryOidcClientSettings>(ctx.Configuration.GetSection("GalleryOidcClient"));
        }
    }
}