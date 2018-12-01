using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using ImageGallery.Client.Settings;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private TokenProviderSettings TokenProviderSettings { get; }

        public Startup(IConfiguration configuration, IOptions<TokenProviderSettings> tokenProviderSettings)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            TokenProviderSettings = tokenProviderSettings.Value;
        }
 
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "CanOrderFrame",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("country", "be");
                        policyBuilder.RequireClaim("subscriptionlevel", "PayingUser");
                    });
            });

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<ImageGalleryApiSettings>(Configuration.GetSection("ImageGalleryApi"));

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies",
              (options) =>
              {
                  options.AccessDeniedPath = "/Authorization/AccessDenied";
              })
              .AddOpenIdConnect("oidc", options =>
                {
                  options.SignInScheme = "Cookies";
                  // note: assume TLS offloading in reverse proxy
                  options.RequireHttpsMetadata = false;
                  options.Authority = TokenProviderSettings.BaseUrl;
                  options.ClientId = "imagegalleryclient";
                  options.ResponseType = "code id_token";
                  //options.CallbackPath = new PathString("...")
                  //options.SignedOutCallbackPath = new PathString("...")
                  options.Scope.Add("openid");
                  options.Scope.Add("profile");
                  options.Scope.Add("address");
                  options.Scope.Add("roles");
                  options.Scope.Add("subscriptionlevel");
                  options.Scope.Add("country");
                  options.Scope.Add("imagegalleryapi");
                  options.Scope.Add("offline_access");
                  options.SaveTokens = true;
                  options.ClientSecret = "secret";
                  options.GetClaimsFromUserInfoEndpoint = true;
                  options.ClaimActions.Remove("amr");
                  options.ClaimActions.DeleteClaim("sid");
                  options.ClaimActions.DeleteClaim("idp");
                  //options.ClaimActions.DeleteClaim("address");
                  options.ClaimActions.MapUniqueJsonKey("role", "role");
                  options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                  options.ClaimActions.MapUniqueJsonKey("country", "country");


                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      NameClaimType = JwtClaimTypes.GivenName,
                      RoleClaimType = JwtClaimTypes.Role,
                  };

              });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }

        public static void ConfigureStartupServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            services.Configure<TokenProviderSettings>(ctx.Configuration.GetSection("TokenProvider"));
        }
    }
}
