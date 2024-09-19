using JWTServiceCore.Data;
using Microsoft.AspNetCore.Identity;
using JWTServiceCore.Models.Entities;

namespace JWTServiceCore.Configurations.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddRoles<ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // User settings.
                options.User.RequireUniqueEmail = true;

                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;

                // SignIn settings.
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = true;

                // Lockout settings.
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.Stores.ProtectPersonalData = false;
            });

            // Configure application cookie
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/LoginFailed";

                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);

                // Prevent redirection in API requests
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = 401; // Unauthorized for APIs
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri); // Redirect to login for web
                    }

                    return Task.CompletedTask;
                };
            });

            return services;
        }
    }
}
