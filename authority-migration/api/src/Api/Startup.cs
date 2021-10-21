using Api.Authentication;
using Api.Configuration;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAuthentication;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthenticationOptions = Api.Authentication.AuthenticationOptions;

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
                })
                .AddFirebaseAuthentication(firebaseOptions.ProjectId, firebaseScheme);

            services.AddTransient<IClaimsTransformation, InternalClaimsTransformation>();

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
