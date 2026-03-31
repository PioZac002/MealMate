using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MealMate.Application.Interfaces;
using MealMate.Application.Services;
using MealMate.Domain.Entities;
using MealMate.Infrastructure.Data;
using MealMate.Infrastructure.Email;

namespace MealMate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IIngredientDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IRecipeDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IFridgeDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IShoppingDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<INutritionDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IFitnessDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
            };
        });

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}
