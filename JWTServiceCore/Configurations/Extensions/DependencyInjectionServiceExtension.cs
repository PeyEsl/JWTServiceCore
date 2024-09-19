using JWTServiceCore.Services.Interfaces;
using JWTServiceCore.Services;

namespace JWTServiceCore.Configurations.Extensions
{
    public static class DependencyInjectionServiceExtension
    {
        public static IServiceCollection AddApplicationScoped(this IServiceCollection services)
        {
            // Add application services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<IReCaptchaService, ReCaptchaService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
