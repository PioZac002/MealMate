using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MuscleGroup MuscleGroup { get; set; }
    public string? Description { get; set; }
    public decimal CaloriesPerMinute { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<WorkoutPlanExercise> WorkoutPlanExercises { get; set; } = new List<WorkoutPlanExercise>();
    public ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
}

public class WorkoutPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public ICollection<WorkoutPlanExercise> Exercises { get; set; } = new List<WorkoutPlanExercise>();
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}

public class WorkoutPlanExercise
{
    public Guid Id { get; set; }
    public Guid WorkoutPlanId { get; set; }
    public WorkoutPlan WorkoutPlan { get; set; } = null!;
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public int RestSeconds { get; set; }
    public int OrderIndex { get; set; }
}

public class Workout
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public DateOnly Date { get; set; }
    public Guid? WorkoutPlanId { get; set; }
    public WorkoutPlan? WorkoutPlan { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }
    public string? Notes { get; set; }

    public ICollection<WorkoutSet> Sets { get; set; } = new List<WorkoutSet>();
}

public class WorkoutSet
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public decimal Weight { get; set; }
    public bool IsPersonalRecord { get; set; } = false;
}
