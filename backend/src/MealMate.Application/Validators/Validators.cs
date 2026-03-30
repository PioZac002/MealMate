using FluentValidation;
using MealMate.Application.DTOs.Auth;
using MealMate.Application.DTOs.Household;
using MealMate.Application.DTOs.Ingredient;
using MealMate.Application.DTOs.Recipe;

namespace MealMate.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
    }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class CreateHouseholdValidator : AbstractValidator<CreateHouseholdDto>
{
    public CreateHouseholdValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class InviteMemberValidator : AbstractValidator<InviteMemberDto>
{
    public InviteMemberValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class JoinHouseholdValidator : AbstractValidator<JoinHouseholdDto>
{
    public JoinHouseholdValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(5);
    }
}

public class CreateIngredientValidator : AbstractValidator<CreateIngredientDto>
{
    public CreateIngredientValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DefaultUnit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CaloriesPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProteinPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CarbsPer100g).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FatPer100g).GreaterThanOrEqualTo(0);
    }
}

public class CreateRecipeValidator : AbstractValidator<CreateRecipeDto>
{
    public CreateRecipeValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PrepTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CookTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Servings).GreaterThan(0);
        RuleFor(x => x.Ingredients).NotEmpty().WithMessage("Recipe must have at least one ingredient.");
        RuleForEach(x => x.Ingredients).ChildRules(i =>
        {
            i.RuleFor(x => x.IngredientId).NotEmpty();
            i.RuleFor(x => x.Quantity).GreaterThan(0);
            i.RuleFor(x => x.Unit).NotEmpty();
        });
        RuleForEach(x => x.Steps).ChildRules(s =>
        {
            s.RuleFor(x => x.StepNumber).GreaterThan(0);
            s.RuleFor(x => x.Description).NotEmpty();
        });
    }
}
