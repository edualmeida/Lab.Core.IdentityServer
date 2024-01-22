using System.Security.Claims;
using IdentityModel;
using Lab.Core.IdentityServer.Data;
using Lab.Core.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Lab.Core.IdentityServer.Data.Initialization;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();

            // Default roles
            context.Roles.Add(new IdentityRole()
            {
                Id = "1",
                Name = "User",
                NormalizedName = "User",
            });

            // Default roles
            context.Roles.Add(new IdentityRole()
            {
                Id = "2",
                Name = "Admin",
                NormalizedName = "Admin",
            });

            context.SaveChanges();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "developer.dublin@gmail.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                userMgr.AddToRoleAsync(alice, "Admin").Wait();

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Admin"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Admin"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            //var bob = userMgr.FindByNameAsync("bob").Result;
            //if (bob == null)
            //{
            //    bob = new ApplicationUser
            //    {
            //        UserName = "bob",
            //        Email = "BobSmith@email.com",
            //        EmailConfirmed = true
            //    };
            //    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
            //    if (!result.Succeeded)
            //    {
            //        throw new Exception(result.Errors.First().Description);
            //    }

            //    result = userMgr.AddClaimsAsync(bob, new Claim[]{
            //                new Claim(JwtClaimTypes.Name, "Bob Smith"),
            //                new Claim(JwtClaimTypes.GivenName, "Bob"),
            //                new Claim(JwtClaimTypes.FamilyName, "Smith"),
            //                new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
            //                new Claim("location", "somewhere")
            //            }).Result;
            //    if (!result.Succeeded)
            //    {
            //        throw new Exception(result.Errors.First().Description);
            //    }
            //    Log.Debug("bob created");
            //}
            //else
            //{
            //    Log.Debug("bob already exists");
            //}
        }
    }
}
