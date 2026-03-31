using AutoMapper;
using MealMate.Application.DTOs.Auth;
using MealMate.Application.DTOs.Fitness;
using MealMate.Application.DTOs.Fridge;
using MealMate.Application.DTOs.Household;
using MealMate.Application.DTOs.Ingredient;
using MealMate.Application.DTOs.Nutrition;
using MealMate.Application.DTOs.Recipe;
using MealMate.Application.DTOs.Shopping;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth
        CreateMap<ApplicationUser, UserDto>();

        // Household
        CreateMap<Household, HouseholdDto>()
            .ForMember(d => d.CreatedByUserName, o => o.MapFrom(s =>
                s.CreatedByUser != null ? $"{s.CreatedByUser.FirstName} {s.CreatedByUser.LastName}" : string.Empty))
            .ForMember(d => d.MemberCount, o => o.MapFrom(s => s.Members.Count));

        CreateMap<Household, HouseholdDetailDto>()
            .ForMember(d => d.CreatedByUserName, o => o.MapFrom(s =>
                s.CreatedByUser != null ? $"{s.CreatedByUser.FirstName} {s.CreatedByUser.LastName}" : string.Empty))
            .ForMember(d => d.MemberCount, o => o.MapFrom(s => s.Members.Count))
            .ForMember(d => d.Members, o => o.MapFrom(s => s.Members));

        CreateMap<HouseholdMember, HouseholdMemberDto>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User != null ? s.User.Email : string.Empty))
            .ForMember(d => d.FullName, o => o.MapFrom(s =>
                s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
            .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.User != null ? s.User.AvatarUrl : null));

        CreateMap<InviteCode, InviteCodeDto>()
            .ForMember(d => d.HouseholdName, o => o.MapFrom(s =>
                s.Household != null ? s.Household.Name : string.Empty));

        // Ingredient
        CreateMap<Ingredient, IngredientDto>();
        CreateMap<CreateIngredientDto, Ingredient>();
        CreateMap<UpdateIngredientDto, Ingredient>();

        // Recipe
        CreateMap<Recipe, RecipeDto>()
            .ForMember(d => d.CreatedByUserName, o => o.MapFrom(s =>
                s.CreatedByUser != null ? $"{s.CreatedByUser.FirstName} {s.CreatedByUser.LastName}" : string.Empty));

        CreateMap<Recipe, RecipeDetailDto>()
            .ForMember(d => d.CreatedByUserName, o => o.MapFrom(s =>
                s.CreatedByUser != null ? $"{s.CreatedByUser.FirstName} {s.CreatedByUser.LastName}" : string.Empty))
            .ForMember(d => d.Ingredients, o => o.MapFrom(s => s.Ingredients))
            .ForMember(d => d.Steps, o => o.MapFrom(s => s.Steps))
            .ForMember(d => d.TotalCalories, o => o.Ignore())
            .ForMember(d => d.TotalProtein, o => o.Ignore())
            .ForMember(d => d.TotalCarbs, o => o.Ignore())
            .ForMember(d => d.TotalFat, o => o.Ignore());

        CreateMap<RecipeIngredient, RecipeIngredientDto>()
            .ForMember(d => d.IngredientName, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.Name : string.Empty))
            .ForMember(d => d.CaloriesPer100g, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.CaloriesPer100g : 0));

        CreateMap<RecipeStep, RecipeStepDto>();

        CreateMap<CreateRecipeDto, Recipe>()
            .ForMember(d => d.Ingredients, o => o.Ignore())
            .ForMember(d => d.Steps, o => o.Ignore());

        CreateMap<CreateRecipeIngredientDto, RecipeIngredient>();
        CreateMap<CreateRecipeStepDto, RecipeStep>();

        // Fridge
        CreateMap<FridgeItem, FridgeItemDto>()
            .ForMember(d => d.IngredientName, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.Name : string.Empty))
            .ForMember(d => d.IngredientCategory, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.Category.ToString() : string.Empty))
            .ForMember(d => d.IngredientImageUrl, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.ImageUrl : null))
            .ForMember(d => d.Source, o => o.MapFrom(s => s.Source.ToString()))
            .ForMember(d => d.AddedByUserName, o => o.MapFrom(s =>
                s.AddedByUser != null ? $"{s.AddedByUser.FirstName} {s.AddedByUser.LastName}" : string.Empty))
            .ForMember(d => d.IsExpiringSoon, o => o.Ignore())
            .ForMember(d => d.IsExpired, o => o.Ignore());

        // Shopping
        CreateMap<ShoppingList, ShoppingListDto>()
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.Items.Count))
            .ForMember(d => d.BoughtCount, o => o.MapFrom(s => s.Items.Count(i => i.IsBought)));

        CreateMap<ShoppingList, ShoppingListDetailDto>()
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

        CreateMap<ShoppingListItem, ShoppingListItemDto>()
            .ForMember(d => d.IngredientName, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.Name : string.Empty))
            .ForMember(d => d.IngredientImageUrl, o => o.MapFrom(s =>
                s.Ingredient != null ? s.Ingredient.ImageUrl : null))
            .ForMember(d => d.Source, o => o.MapFrom(s => s.Source.ToString()));

        // Nutrition
        CreateMap<DailyNutritionLog, DailyNutritionLogDto>()
            .ForMember(d => d.MealLogs, o => o.MapFrom(s => s.MealLogs))
            .ForMember(d => d.TotalCalories, o => o.Ignore())
            .ForMember(d => d.TotalProtein, o => o.Ignore())
            .ForMember(d => d.TotalCarbs, o => o.Ignore())
            .ForMember(d => d.TotalFat, o => o.Ignore());

        CreateMap<MealLog, MealLogDto>()
            .ForMember(d => d.MealType, o => o.MapFrom(s => s.MealType.ToString()))
            .ForMember(d => d.RecipeName, o => o.MapFrom(s => s.Recipe != null ? s.Recipe.Title : null))
            .ForMember(d => d.FoodName, o => o.Ignore());

        // Fitness
        CreateMap<Exercise, ExerciseDto>()
            .ForMember(d => d.MuscleGroup, o => o.MapFrom(s => s.MuscleGroup.ToString()));

        CreateMap<WorkoutPlan, WorkoutPlanDto>()
            .ForMember(d => d.ExerciseCount, o => o.MapFrom(s => s.Exercises.Count));

        CreateMap<WorkoutPlan, WorkoutPlanDetailDto>()
            .ForMember(d => d.Exercises, o => o.MapFrom(s => s.Exercises));

        CreateMap<WorkoutPlanExercise, WorkoutPlanExerciseDto>()
            .ForMember(d => d.ExerciseName, o => o.MapFrom(s =>
                s.Exercise != null ? s.Exercise.Name : string.Empty))
            .ForMember(d => d.MuscleGroup, o => o.MapFrom(s =>
                s.Exercise != null ? s.Exercise.MuscleGroup.ToString() : string.Empty));

        CreateMap<Workout, WorkoutDto>()
            .ForMember(d => d.WorkoutPlanName, o => o.MapFrom(s =>
                s.WorkoutPlan != null ? s.WorkoutPlan.Name : null))
            .ForMember(d => d.SetCount, o => o.MapFrom(s => s.Sets.Count));

        CreateMap<Workout, WorkoutDetailDto>()
            .ForMember(d => d.WorkoutPlanName, o => o.MapFrom(s =>
                s.WorkoutPlan != null ? s.WorkoutPlan.Name : null))
            .ForMember(d => d.Sets, o => o.MapFrom(s => s.Sets));

        CreateMap<WorkoutSet, WorkoutSetDto>()
            .ForMember(d => d.ExerciseName, o => o.MapFrom(s =>
                s.Exercise != null ? s.Exercise.Name : string.Empty))
            .ForMember(d => d.MuscleGroup, o => o.MapFrom(s =>
                s.Exercise != null ? s.Exercise.MuscleGroup.ToString() : string.Empty));
    }
}
