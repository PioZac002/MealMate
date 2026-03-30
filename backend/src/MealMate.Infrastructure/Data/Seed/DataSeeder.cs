using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            await context.Database.MigrateAsync();
            await SeedRolesAsync(roleManager);
            await SeedIngredientsAsync(context);
            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database.");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role) { Id = Guid.NewGuid() });
        }
    }

    private static async Task SeedIngredientsAsync(AppDbContext context)
    {
        if (await context.Ingredients.AnyAsync()) return;

        var ingredients = new List<Ingredient>
        {
            new() { Id = Guid.NewGuid(), Name = "Chicken Breast", DefaultUnit = "g", Category = IngredientCategory.Meat, CaloriesPer100g = 165, ProteinPer100g = 31, CarbsPer100g = 0, FatPer100g = 3.6m },
            new() { Id = Guid.NewGuid(), Name = "Salmon", DefaultUnit = "g", Category = IngredientCategory.Meat, CaloriesPer100g = 208, ProteinPer100g = 20, CarbsPer100g = 0, FatPer100g = 13 },
            new() { Id = Guid.NewGuid(), Name = "Beef (Ground)", DefaultUnit = "g", Category = IngredientCategory.Meat, CaloriesPer100g = 250, ProteinPer100g = 26, CarbsPer100g = 0, FatPer100g = 15 },
            new() { Id = Guid.NewGuid(), Name = "Eggs", DefaultUnit = "pcs", Category = IngredientCategory.Dairy, CaloriesPer100g = 155, ProteinPer100g = 13, CarbsPer100g = 1.1m, FatPer100g = 11 },
            new() { Id = Guid.NewGuid(), Name = "Whole Milk", DefaultUnit = "ml", Category = IngredientCategory.Dairy, CaloriesPer100g = 61, ProteinPer100g = 3.2m, CarbsPer100g = 4.8m, FatPer100g = 3.3m },
            new() { Id = Guid.NewGuid(), Name = "Cheddar Cheese", DefaultUnit = "g", Category = IngredientCategory.Dairy, CaloriesPer100g = 402, ProteinPer100g = 25, CarbsPer100g = 1.3m, FatPer100g = 33 },
            new() { Id = Guid.NewGuid(), Name = "Greek Yogurt", DefaultUnit = "g", Category = IngredientCategory.Dairy, CaloriesPer100g = 59, ProteinPer100g = 10, CarbsPer100g = 3.6m, FatPer100g = 0.4m },
            new() { Id = Guid.NewGuid(), Name = "Butter", DefaultUnit = "g", Category = IngredientCategory.Dairy, CaloriesPer100g = 717, ProteinPer100g = 0.9m, CarbsPer100g = 0.1m, FatPer100g = 81 },
            new() { Id = Guid.NewGuid(), Name = "White Rice", DefaultUnit = "g", Category = IngredientCategory.Grains, CaloriesPer100g = 130, ProteinPer100g = 2.7m, CarbsPer100g = 28, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Brown Rice", DefaultUnit = "g", Category = IngredientCategory.Grains, CaloriesPer100g = 216, ProteinPer100g = 5, CarbsPer100g = 45, FatPer100g = 1.8m },
            new() { Id = Guid.NewGuid(), Name = "Pasta (Spaghetti)", DefaultUnit = "g", Category = IngredientCategory.Grains, CaloriesPer100g = 371, ProteinPer100g = 13, CarbsPer100g = 75, FatPer100g = 1.5m },
            new() { Id = Guid.NewGuid(), Name = "Wheat Flour", DefaultUnit = "g", Category = IngredientCategory.Grains, CaloriesPer100g = 364, ProteinPer100g = 10, CarbsPer100g = 76, FatPer100g = 1 },
            new() { Id = Guid.NewGuid(), Name = "Oats", DefaultUnit = "g", Category = IngredientCategory.Grains, CaloriesPer100g = 389, ProteinPer100g = 17, CarbsPer100g = 66, FatPer100g = 7 },
            new() { Id = Guid.NewGuid(), Name = "Bread (White)", DefaultUnit = "g", Category = IngredientCategory.Grains, CaloriesPer100g = 265, ProteinPer100g = 9, CarbsPer100g = 49, FatPer100g = 3.2m },
            new() { Id = Guid.NewGuid(), Name = "Tomatoes", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 18, ProteinPer100g = 0.9m, CarbsPer100g = 3.9m, FatPer100g = 0.2m },
            new() { Id = Guid.NewGuid(), Name = "Onion", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 40, ProteinPer100g = 1.1m, CarbsPer100g = 9.3m, FatPer100g = 0.1m },
            new() { Id = Guid.NewGuid(), Name = "Garlic", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 149, ProteinPer100g = 6.4m, CarbsPer100g = 33, FatPer100g = 0.5m },
            new() { Id = Guid.NewGuid(), Name = "Broccoli", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 34, ProteinPer100g = 2.8m, CarbsPer100g = 7, FatPer100g = 0.4m },
            new() { Id = Guid.NewGuid(), Name = "Spinach", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 23, ProteinPer100g = 2.9m, CarbsPer100g = 3.6m, FatPer100g = 0.4m },
            new() { Id = Guid.NewGuid(), Name = "Carrots", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 41, ProteinPer100g = 0.9m, CarbsPer100g = 10, FatPer100g = 0.2m },
            new() { Id = Guid.NewGuid(), Name = "Bell Pepper (Red)", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 31, ProteinPer100g = 1, CarbsPer100g = 6, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Cucumber", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 16, ProteinPer100g = 0.7m, CarbsPer100g = 3.6m, FatPer100g = 0.1m },
            new() { Id = Guid.NewGuid(), Name = "Potatoes", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 77, ProteinPer100g = 2, CarbsPer100g = 17, FatPer100g = 0.1m },
            new() { Id = Guid.NewGuid(), Name = "Mushrooms", DefaultUnit = "g", Category = IngredientCategory.Vegetables, CaloriesPer100g = 22, ProteinPer100g = 3.1m, CarbsPer100g = 3.3m, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Apple", DefaultUnit = "pcs", Category = IngredientCategory.Fruit, CaloriesPer100g = 52, ProteinPer100g = 0.3m, CarbsPer100g = 14, FatPer100g = 0.2m },
            new() { Id = Guid.NewGuid(), Name = "Banana", DefaultUnit = "pcs", Category = IngredientCategory.Fruit, CaloriesPer100g = 89, ProteinPer100g = 1.1m, CarbsPer100g = 23, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Orange", DefaultUnit = "pcs", Category = IngredientCategory.Fruit, CaloriesPer100g = 47, ProteinPer100g = 0.9m, CarbsPer100g = 12, FatPer100g = 0.1m },
            new() { Id = Guid.NewGuid(), Name = "Strawberries", DefaultUnit = "g", Category = IngredientCategory.Fruit, CaloriesPer100g = 32, ProteinPer100g = 0.7m, CarbsPer100g = 7.7m, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Blueberries", DefaultUnit = "g", Category = IngredientCategory.Fruit, CaloriesPer100g = 57, ProteinPer100g = 0.7m, CarbsPer100g = 14, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Lemon", DefaultUnit = "pcs", Category = IngredientCategory.Fruit, CaloriesPer100g = 29, ProteinPer100g = 1.1m, CarbsPer100g = 9.3m, FatPer100g = 0.3m },
            new() { Id = Guid.NewGuid(), Name = "Olive Oil", DefaultUnit = "ml", Category = IngredientCategory.Other, CaloriesPer100g = 884, ProteinPer100g = 0, CarbsPer100g = 0, FatPer100g = 100 },
            new() { Id = Guid.NewGuid(), Name = "Salt", DefaultUnit = "g", Category = IngredientCategory.Spices, CaloriesPer100g = 0, ProteinPer100g = 0, CarbsPer100g = 0, FatPer100g = 0 },
            new() { Id = Guid.NewGuid(), Name = "Black Pepper", DefaultUnit = "g", Category = IngredientCategory.Spices, CaloriesPer100g = 251, ProteinPer100g = 10, CarbsPer100g = 64, FatPer100g = 3.3m },
            new() { Id = Guid.NewGuid(), Name = "Cumin", DefaultUnit = "g", Category = IngredientCategory.Spices, CaloriesPer100g = 375, ProteinPer100g = 18, CarbsPer100g = 44, FatPer100g = 22 },
            new() { Id = Guid.NewGuid(), Name = "Paprika", DefaultUnit = "g", Category = IngredientCategory.Spices, CaloriesPer100g = 282, ProteinPer100g = 14, CarbsPer100g = 54, FatPer100g = 13 },
            new() { Id = Guid.NewGuid(), Name = "Canned Tomatoes", DefaultUnit = "g", Category = IngredientCategory.Other, CaloriesPer100g = 24, ProteinPer100g = 1.2m, CarbsPer100g = 5, FatPer100g = 0.2m },
            new() { Id = Guid.NewGuid(), Name = "Chicken Broth", DefaultUnit = "ml", Category = IngredientCategory.Other, CaloriesPer100g = 15, ProteinPer100g = 1.8m, CarbsPer100g = 1.5m, FatPer100g = 0.5m },
            new() { Id = Guid.NewGuid(), Name = "Soy Sauce", DefaultUnit = "ml", Category = IngredientCategory.Other, CaloriesPer100g = 53, ProteinPer100g = 8.1m, CarbsPer100g = 4.9m, FatPer100g = 0.1m },
            new() { Id = Guid.NewGuid(), Name = "Honey", DefaultUnit = "g", Category = IngredientCategory.Other, CaloriesPer100g = 304, ProteinPer100g = 0.3m, CarbsPer100g = 82, FatPer100g = 0 },
            new() { Id = Guid.NewGuid(), Name = "Avocado", DefaultUnit = "pcs", Category = IngredientCategory.Fruit, CaloriesPer100g = 160, ProteinPer100g = 2, CarbsPer100g = 9, FatPer100g = 15 },
        };

        context.Ingredients.AddRange(ingredients);
        await context.SaveChangesAsync();
    }
}
