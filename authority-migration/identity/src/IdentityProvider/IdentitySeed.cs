using IdentityModel;
using IdentityProvider.Infrastructure;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityProvider.Infrastructure.Database;

namespace IdentityProvider
{
    public class IdentitySeed
    {
        public static async Task EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            await using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var jane = await userManager.FindByNameAsync("jane");
            
            if (jane == null)
            {
                jane = new ApplicationUser
                {
                    UserName = "jane",
                    Email = "jane.doe@email.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(jane, "Str0ngPassword!");
                
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userManager.AddClaimsAsync(jane, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Jane Doe"),
                    new Claim(JwtClaimTypes.GivenName, "Jane"),
                    new Claim(JwtClaimTypes.FamilyName, "Doe"),
                    new Claim(JwtClaimTypes.WebSite, "http://jane.com"),
                });

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
    }
}
