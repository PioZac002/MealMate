using MealMate.Domain.Entities;
using MealMate.Domain.Enums;
using Xunit;

namespace MealMate.Tests;

public class DomainEntityTests
{
    [Fact]
    public void Household_NewInstance_ShouldHaveEmptyMembersCollection()
    {
        var household = new Household();

        Assert.NotNull(household.Members);
        Assert.Empty(household.Members);
    }

    [Fact]
    public void Recipe_NewInstance_ShouldHaveEmptyCollections()
    {
        var recipe = new Recipe();

        Assert.NotNull(recipe.Ingredients);
        Assert.Empty(recipe.Ingredients);
        Assert.NotNull(recipe.Steps);
        Assert.Empty(recipe.Steps);
    }

    [Fact]
    public void Ingredient_DecimalProperties_ShouldDefaultToZero()
    {
        var ingredient = new Ingredient();

        Assert.Equal(0, ingredient.CaloriesPer100g);
        Assert.Equal(0, ingredient.ProteinPer100g);
        Assert.Equal(0, ingredient.CarbsPer100g);
        Assert.Equal(0, ingredient.FatPer100g);
    }

    [Fact]
    public void HouseholdMember_DefaultRole_ShouldBeMember()
    {
        var member = new HouseholdMember();

        Assert.Equal(HouseholdRole.Member, member.Role);
    }

    [Fact]
    public void InviteCode_NewInstance_ShouldNotBeUsed()
    {
        var invite = new InviteCode();

        Assert.False(invite.IsUsed);
    }

    [Fact]
    public void FridgeItem_DefaultSource_ShouldBeManual()
    {
        var item = new FridgeItem();

        Assert.Equal(FridgeItemSource.Manual, item.Source);
    }

    [Fact]
    public void ShoppingListItem_DefaultState_ShouldNotBeBought()
    {
        var item = new ShoppingListItem();

        Assert.False(item.IsBought);
        Assert.Equal(ShoppingItemSource.Manual, item.Source);
    }
}
