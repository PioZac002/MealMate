using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MealMate.Application.Services;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>,
    IApplicationDbContext, IIngredientDbContext, IRecipeDbContext, IFridgeDbContext, IShoppingDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Household> Households => Set<Household>();
    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();
    public DbSet<InviteCode> InviteCodes => Set<InviteCode>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<MealPlanEntry> MealPlanEntries => Set<MealPlanEntry>();
    public DbSet<FridgeItem> FridgeItems => Set<FridgeItem>();
    public DbSet<MustHaveProduct> MustHaveProducts => Set<MustHaveProduct>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<DailyNutritionLog> DailyNutritionLogs => Set<DailyNutritionLog>();
    public DbSet<MealLog> MealLogs => Set<MealLog>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();
    public DbSet<WorkoutPlanExercise> WorkoutPlanExercises => Set<WorkoutPlanExercise>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity table names
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // HouseholdMember composite key
        builder.Entity<HouseholdMember>()
            .HasKey(m => new { m.UserId, m.HouseholdId });

        builder.Entity<HouseholdMember>()
            .HasOne(m => m.User)
            .WithMany(u => u.HouseholdMemberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<HouseholdMember>()
            .HasOne(m => m.Household)
            .WithMany(h => h.Members)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        // Household
        builder.Entity<Household>()
            .HasOne(h => h.CreatedByUser)
            .WithMany(u => u.CreatedHouseholds)
            .HasForeignKey(h => h.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // InviteCode
        builder.Entity<InviteCode>()
            .HasOne(i => i.Household)
            .WithMany(h => h.InviteCodes)
            .HasForeignKey(i => i.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<InviteCode>()
            .HasOne(i => i.CreatedByUser)
            .WithMany(u => u.CreatedInvites)
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InviteCode>()
            .HasIndex(i => i.Code);

        // Ingredient
        builder.Entity<Ingredient>()
            .Property(i => i.CaloriesPer100g).HasColumnType("decimal(10,2)");
        builder.Entity<Ingredient>()
            .Property(i => i.ProteinPer100g).HasColumnType("decimal(10,2)");
        builder.Entity<Ingredient>()
            .Property(i => i.CarbsPer100g).HasColumnType("decimal(10,2)");
        builder.Entity<Ingredient>()
            .Property(i => i.FatPer100g).HasColumnType("decimal(10,2)");

        // Recipe
        builder.Entity<Recipe>()
            .HasOne(r => r.CreatedByUser)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // RecipeIngredient
        builder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RecipeIngredient>()
            .Property(ri => ri.Quantity).HasColumnType("decimal(10,2)");

        // RecipeStep
        builder.Entity<RecipeStep>()
            .HasOne(s => s.Recipe)
            .WithMany(r => r.Steps)
            .HasForeignKey(s => s.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // FridgeItem
        builder.Entity<FridgeItem>()
            .HasOne(f => f.Household)
            .WithMany(h => h.FridgeItems)
            .HasForeignKey(f => f.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FridgeItem>()
            .HasOne(f => f.AddedByUser)
            .WithMany(u => u.FridgeItems)
            .HasForeignKey(f => f.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FridgeItem>()
            .Property(f => f.Quantity).HasColumnType("decimal(10,2)");

        // MustHaveProduct
        builder.Entity<MustHaveProduct>()
            .HasOne(m => m.Household)
            .WithMany(h => h.MustHaveProducts)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MustHaveProduct>()
            .Property(m => m.MinQuantity).HasColumnType("decimal(10,2)");

        // ShoppingList
        builder.Entity<ShoppingList>()
            .HasOne(s => s.Household)
            .WithMany(h => h.ShoppingLists)
            .HasForeignKey(s => s.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        // ShoppingListItem
        builder.Entity<ShoppingListItem>()
            .HasOne(i => i.ShoppingList)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ShoppingListItem>()
            .HasOne(i => i.BoughtByUser)
            .WithMany()
            .HasForeignKey(i => i.BoughtByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ShoppingListItem>()
            .Property(i => i.Quantity).HasColumnType("decimal(10,2)");

        // Receipt
        builder.Entity<Receipt>()
            .HasOne(r => r.User)
            .WithMany(u => u.Receipts)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Receipt>()
            .HasOne(r => r.Household)
            .WithMany(h => h.Receipts)
            .HasForeignKey(r => r.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Receipt>()
            .Property(r => r.TotalAmount).HasColumnType("decimal(10,2)");

        builder.Entity<ReceiptItem>()
            .HasOne(i => i.Receipt)
            .WithMany(r => r.Items)
            .HasForeignKey(i => i.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReceiptItem>()
            .HasOne(i => i.Ingredient)
            .WithMany()
            .HasForeignKey(i => i.IngredientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ReceiptItem>()
            .Property(i => i.Quantity).HasColumnType("decimal(10,2)");
        builder.Entity<ReceiptItem>()
            .Property(i => i.Price).HasColumnType("decimal(10,2)");

        // DailyNutritionLog
        builder.Entity<DailyNutritionLog>()
            .HasOne(d => d.User)
            .WithMany(u => u.NutritionLogs)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DailyNutritionLog>()
            .HasIndex(d => new { d.UserId, d.Date }).IsUnique();

        builder.Entity<DailyNutritionLog>()
            .Property(d => d.CalorieGoal).HasColumnType("decimal(10,2)");
        builder.Entity<DailyNutritionLog>()
            .Property(d => d.ProteinGoal).HasColumnType("decimal(10,2)");
        builder.Entity<DailyNutritionLog>()
            .Property(d => d.CarbsGoal).HasColumnType("decimal(10,2)");
        builder.Entity<DailyNutritionLog>()
            .Property(d => d.FatGoal).HasColumnType("decimal(10,2)");

        // MealLog
        builder.Entity<MealLog>()
            .HasOne(m => m.DailyNutritionLog)
            .WithMany(d => d.MealLogs)
            .HasForeignKey(m => m.DailyNutritionLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MealLog>()
            .HasOne(m => m.Recipe)
            .WithMany(r => r.MealLogs)
            .HasForeignKey(m => m.RecipeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MealLog>()
            .Property(m => m.Calories).HasColumnType("decimal(10,2)");
        builder.Entity<MealLog>()
            .Property(m => m.Protein).HasColumnType("decimal(10,2)");
        builder.Entity<MealLog>()
            .Property(m => m.Carbs).HasColumnType("decimal(10,2)");
        builder.Entity<MealLog>()
            .Property(m => m.Fat).HasColumnType("decimal(10,2)");
        builder.Entity<MealLog>()
            .Property(m => m.Servings).HasColumnType("decimal(10,2)");

        // Workout
        builder.Entity<Workout>()
            .HasOne(w => w.User)
            .WithMany(u => u.Workouts)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Workout>()
            .HasOne(w => w.WorkoutPlan)
            .WithMany(p => p.Workouts)
            .HasForeignKey(w => w.WorkoutPlanId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Workout>()
            .Property(w => w.CaloriesBurned).HasColumnType("decimal(10,2)");

        // WorkoutPlan
        builder.Entity<WorkoutPlan>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.WorkoutPlans)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // WorkoutPlanExercise
        builder.Entity<WorkoutPlanExercise>()
            .HasOne(e => e.WorkoutPlan)
            .WithMany(p => p.Exercises)
            .HasForeignKey(e => e.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WorkoutPlanExercise>()
            .HasOne(e => e.Exercise)
            .WithMany(ex => ex.WorkoutPlanExercises)
            .HasForeignKey(e => e.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        // WorkoutSet
        builder.Entity<WorkoutSet>()
            .HasOne(s => s.Workout)
            .WithMany(w => w.Sets)
            .HasForeignKey(s => s.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WorkoutSet>()
            .HasOne(s => s.Exercise)
            .WithMany(e => e.WorkoutSets)
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WorkoutSet>()
            .Property(s => s.Weight).HasColumnType("decimal(10,2)");
    }
}
