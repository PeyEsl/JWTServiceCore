using Microsoft.OpenApi.Models;

namespace JWTServiceCore.Configurations.Extensions
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddApplicationSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Authentication Core",
                    Description = "Authentication - ASP.NET Core",
                    TermsOfService = null,
                    Contact = new OpenApiContact()
                    {
                        Name = "Peyman Eslami",
                        Email = "artan.co.5827@gmail.com",
                        Url = new Uri("http://www.peyesl-artan.ir"),
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT. Example: Bearer {your token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
