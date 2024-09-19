using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTServiceCore.Configurations.Extensions
{
    public static class AuthenticationServiceExtension
    {
        public static IServiceCollection AddApplicationAuthentication(this IServiceCollection services, AppSetting appSetting)
        {
            // JWT Authentication
            var key = Encoding.ASCII.GetBytes(appSetting.Secret!);
            services.AddAuthentication(options =>
            {
                // Cookie Authentication for web
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                // JWT Authentication for API
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // Adding Cookie Authentication for web
                .AddCookie(options =>
                {
                    // Configure application cookie settings
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/LoginFailed";

                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                })
                // Adding JWT Authentication for API
                .AddJwtBearer(options =>
                {
                    // Configure JWT authentication settings
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };

                    // Handle unauthorized API requests without redirecting
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse(); // Prevent the default redirect
                            context.Response.StatusCode = 401; // Return 401 status for APIs
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}