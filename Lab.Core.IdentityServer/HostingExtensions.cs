using Duende.IdentityServer;
using Lab.Core.IdentityServer.Data;
using Lab.Core.IdentityServer.Models;
using Lab.Core.IdentityServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using Duende.IdentityServer.AspNetIdentity;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Core.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        AddInfrastructure(builder);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

        builder.Services.AddRazorPages();
        builder.Services.AddSession(options => {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
        });
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>(options => 
                options.SignIn.RequireConfirmedAccount = true
            )
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            //.AddInMemoryIdentityResources(Config.IdentityResources)
            //.AddInMemoryApiScopes(Config.ApiScopes)
            //.AddInMemoryClients(Config.Clients)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService<ApplicationUser>>();
        
        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }

    private static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(
            new EmailOptions(builder.Configuration["Smtp:FromAddress"], builder.Configuration["Smtp:FromDisplayName"])
            {
                Host = builder.Configuration["Smtp:Host"],
                Port = int.Parse(builder.Configuration["Smtp:Port"]),
                Username = builder.Configuration["Smtp:Username"],
                Password = builder.Configuration["Smtp:Password"],
            });
        builder.Services.AddTransient<IEmailNotifier, EmailNotifier>();
    }
}