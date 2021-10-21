using Api.Configuration;
using FirebaseAdmin;
using FirebaseAuthentication;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Authentication;
using FirebaseAdmin.Auth;
using ClaimTypes = Api.Authentication.ClaimTypes;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var firebaseOptions = Configuration.GetSection("Firebase").Get<FirebaseOptions>();

            services.AddSingleton(c =>
            {
                var credentials = GoogleCredential.FromJson(Encoding.UTF8.GetString(Convert.FromBase64String(firebaseOptions.Credentials)));
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credentials,
                    ProjectId = firebaseOptions.ProjectId
                });

                return FirebaseAuth.DefaultInstance;
            });

            var authenticationOptions = Configuration.GetSection("Authentication").Get<AuthenticationOptions>();

            const string firebaseScheme = "Firebase";

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authenticationOptions.Authority;
                    options.Audience = authenticationOptions.Audience;
                    options.RequireHttpsMetadata = authenticationOptions.RequireHttps;

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var identity = new ClaimsIdentity();

                            // Map unique identifier claims from the old authority to those used in the new one
                            var userId = context.Principal.FindFirst(ClaimTypes.OldAuthorityUserId)?.Value;
                            if (userId == null)
                            {
                                context.Fail("The 'sub' claim is not present in the token.");
                                return Task.CompletedTask;
                            }

                            identity.AddClaim(new Claim(ClaimTypes.UserId, userId));

                            context.Principal.AddIdentity(identity);

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddFirebaseAuthentication(firebaseOptions.ProjectId, firebaseScheme);

            services.AddAuthorization(options =>
            {
                // magic sauce for getting multiple providers to work
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, firebaseScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
