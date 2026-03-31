using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MealMate.Application.Interfaces;
using MealMate.Application.Mappings;
using MealMate.Application.Services;
using MealMate.Application.Validators;

namespace MealMate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHouseholdService, HouseholdService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IFridgeService, FridgeService>();
        services.AddScoped<IShoppingService, ShoppingService>();
        services.AddScoped<INutritionService, NutritionService>();
        services.AddScoped<IFitnessService, FitnessService>();

        return services;
    }
}
