using MealMate.Application.Common;
using MealMate.Application.DTOs.Fitness;

namespace MealMate.Application.Interfaces;

public interface IFitnessService
{
    Task<ServiceResult<IEnumerable<ExerciseDto>>> GetExercisesAsync(string? search, string? muscleGroup);
    Task<ServiceResult<ExerciseDto>> CreateExerciseAsync(CreateExerciseDto dto);
    Task<ServiceResult<IEnumerable<WorkoutPlanDto>>> GetWorkoutPlansAsync(Guid userId);
    Task<ServiceResult<WorkoutPlanDetailDto>> GetWorkoutPlanAsync(Guid planId, Guid userId);
    Task<ServiceResult<WorkoutPlanDetailDto>> CreateWorkoutPlanAsync(CreateWorkoutPlanDto dto, Guid userId);
    Task<ServiceResult<bool>> DeleteWorkoutPlanAsync(Guid planId, Guid userId);
    Task<ServiceResult<IEnumerable<WorkoutDto>>> GetWorkoutsAsync(Guid userId, int days);
    Task<ServiceResult<WorkoutDetailDto>> GetWorkoutAsync(Guid workoutId, Guid userId);
    Task<ServiceResult<WorkoutDetailDto>> LogWorkoutAsync(LogWorkoutDto dto, Guid userId);
    Task<ServiceResult<bool>> DeleteWorkoutAsync(Guid workoutId, Guid userId);
}
