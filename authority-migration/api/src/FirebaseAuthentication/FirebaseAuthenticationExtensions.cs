using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace FirebaseAuthentication
{
    public static class FirebaseAuthenticationExtensions
    {
        public static AuthenticationBuilder AddFirebaseAuthentication(
            this AuthenticationBuilder builder,
            string firebaseProjectId,
            Action<JwtBearerOptions> configure = null)
        {
            return AddFirebaseAuthentication(
                builder,
                firebaseProjectId,
                JwtBearerDefaults.AuthenticationScheme,
                configure);
        }

        public static AuthenticationBuilder AddFirebaseAuthentication(
            this AuthenticationBuilder builder,
            string firebaseProjectId,
            string scheme,
            Action<JwtBearerOptions> configure = null)
        {
            return builder.AddJwtBearer(scheme, options =>
            {
                options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Authority,
                    ValidateAudience = true,
                    ValidAudience = firebaseProjectId,
                    ValidateLifetime = true
                };
                configure?.Invoke(options);
            });
        }
    }
}
