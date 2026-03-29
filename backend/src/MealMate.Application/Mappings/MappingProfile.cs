using AutoMapper;
using MealMate.Application.DTOs.Auth;
using MealMate.Application.DTOs.Household;
using MealMate.Application.DTOs.Ingredient;
using MealMate.Application.DTOs.Recipe;
using MealMate.Domain.Entities;

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
    }
}
