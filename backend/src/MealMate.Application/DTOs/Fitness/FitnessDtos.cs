namespace MealMate.Application.DTOs.Fitness;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CaloriesPerMinute { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CaloriesPerMinute { get; set; }
}

public class WorkoutPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ExerciseCount { get; set; }
}

public class WorkoutPlanDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IEnumerable<WorkoutPlanExerciseDto> Exercises { get; set; } = Enumerable.Empty<WorkoutPlanExerciseDto>();
}

public class WorkoutPlanExerciseDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public int RestSeconds { get; set; }
    public int OrderIndex { get; set; }
}

public class CreateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IEnumerable<CreateWorkoutPlanExerciseDto> Exercises { get; set; } = Enumerable.Empty<CreateWorkoutPlanExerciseDto>();
}

public class CreateWorkoutPlanExerciseDto
{
    public Guid ExerciseId { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public int RestSeconds { get; set; }
    public int OrderIndex { get; set; }
}

public class WorkoutDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public Guid? WorkoutPlanId { get; set; }
    public string? WorkoutPlanName { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public int SetCount { get; set; }
}

public class WorkoutDetailDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public Guid? WorkoutPlanId { get; set; }
    public string? WorkoutPlanName { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<WorkoutSetDto> Sets { get; set; } = Enumerable.Empty<WorkoutSetDto>();
}

public class WorkoutSetDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public decimal Weight { get; set; }
    public bool IsPersonalRecord { get; set; }
}

public class LogWorkoutDto
{
    public DateOnly Date { get; set; }
    public Guid? WorkoutPlanId { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<LogWorkoutSetDto> Sets { get; set; } = Enumerable.Empty<LogWorkoutSetDto>();
}

public class LogWorkoutSetDto
{
    public Guid ExerciseId { get; set; }
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public decimal Weight { get; set; }
}
