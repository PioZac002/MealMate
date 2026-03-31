using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Fitness;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Services;

public interface IFitnessDbContext
{
    DbSet<Exercise> Exercises { get; }
    DbSet<WorkoutPlan> WorkoutPlans { get; }
    DbSet<WorkoutPlanExercise> WorkoutPlanExercises { get; }
    DbSet<Workout> Workouts { get; }
    DbSet<WorkoutSet> WorkoutSets { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class FitnessService : IFitnessService
{
    private readonly IFitnessDbContext _db;
    private readonly IMapper _mapper;

    public FitnessService(IFitnessDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<ExerciseDto>>> GetExercisesAsync(string? search, string? muscleGroup)
    {
        var query = _db.Exercises.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Name.ToLower().Contains(search.ToLower()));

        if (!string.IsNullOrWhiteSpace(muscleGroup) &&
            Enum.TryParse<MuscleGroup>(muscleGroup, ignoreCase: true, out var mg))
            query = query.Where(e => e.MuscleGroup == mg);

        var exercises = await query.OrderBy(e => e.Name).ToListAsync();
        return ServiceResult<IEnumerable<ExerciseDto>>.Ok(_mapper.Map<IEnumerable<ExerciseDto>>(exercises));
    }

    public async Task<ServiceResult<ExerciseDto>> CreateExerciseAsync(CreateExerciseDto dto)
    {
        if (!Enum.TryParse<MuscleGroup>(dto.MuscleGroup, ignoreCase: true, out var mg))
            return ServiceResult<ExerciseDto>.Fail("Invalid muscle group");

        var exercise = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            MuscleGroup = mg,
            Description = dto.Description,
            CaloriesPerMinute = dto.CaloriesPerMinute
        };

        _db.Exercises.Add(exercise);
        await _db.SaveChangesAsync();
        return ServiceResult<ExerciseDto>.Created(_mapper.Map<ExerciseDto>(exercise));
    }

    public async Task<ServiceResult<IEnumerable<WorkoutPlanDto>>> GetWorkoutPlansAsync(Guid userId)
    {
        var plans = await _db.WorkoutPlans
            .Include(p => p.Exercises)
            .Where(p => p.CreatedByUserId == userId)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return ServiceResult<IEnumerable<WorkoutPlanDto>>.Ok(_mapper.Map<IEnumerable<WorkoutPlanDto>>(plans));
    }

    public async Task<ServiceResult<WorkoutPlanDetailDto>> GetWorkoutPlanAsync(Guid planId, Guid userId)
    {
        var plan = await _db.WorkoutPlans
            .Include(p => p.Exercises)
                .ThenInclude(e => e.Exercise)
            .FirstOrDefaultAsync(p => p.Id == planId);

        if (plan == null)
            return ServiceResult<WorkoutPlanDetailDto>.NotFound("Workout plan not found");

        if (plan.CreatedByUserId != userId)
            return ServiceResult<WorkoutPlanDetailDto>.Forbidden();

        return ServiceResult<WorkoutPlanDetailDto>.Ok(_mapper.Map<WorkoutPlanDetailDto>(plan));
    }

    public async Task<ServiceResult<WorkoutPlanDetailDto>> CreateWorkoutPlanAsync(CreateWorkoutPlanDto dto, Guid userId)
    {
        var plan = new WorkoutPlan
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CreatedByUserId = userId
        };

        foreach (var ex in dto.Exercises)
        {
            var exercise = await _db.Exercises.FindAsync(ex.ExerciseId);
            if (exercise == null)
                return ServiceResult<WorkoutPlanDetailDto>.Fail($"Exercise {ex.ExerciseId} not found");

            plan.Exercises.Add(new WorkoutPlanExercise
            {
                Id = Guid.NewGuid(),
                WorkoutPlanId = plan.Id,
                ExerciseId = ex.ExerciseId,
                Sets = ex.Sets,
                Reps = ex.Reps,
                RestSeconds = ex.RestSeconds,
                OrderIndex = ex.OrderIndex
            });
        }

        _db.WorkoutPlans.Add(plan);
        await _db.SaveChangesAsync();

        var created = await _db.WorkoutPlans
            .Include(p => p.Exercises)
                .ThenInclude(e => e.Exercise)
            .FirstAsync(p => p.Id == plan.Id);

        return ServiceResult<WorkoutPlanDetailDto>.Created(_mapper.Map<WorkoutPlanDetailDto>(created));
    }

    public async Task<ServiceResult<bool>> DeleteWorkoutPlanAsync(Guid planId, Guid userId)
    {
        var plan = await _db.WorkoutPlans.FindAsync(planId);
        if (plan == null)
            return ServiceResult<bool>.NotFound("Workout plan not found");
        if (plan.CreatedByUserId != userId)
            return ServiceResult<bool>.Forbidden();

        _db.WorkoutPlans.Remove(plan);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<IEnumerable<WorkoutDto>>> GetWorkoutsAsync(Guid userId, int days)
    {
        var from = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        var workouts = await _db.Workouts
            .Include(w => w.WorkoutPlan)
            .Include(w => w.Sets)
            .Where(w => w.UserId == userId && w.Date >= from)
            .OrderByDescending(w => w.Date)
            .ToListAsync();

        return ServiceResult<IEnumerable<WorkoutDto>>.Ok(_mapper.Map<IEnumerable<WorkoutDto>>(workouts));
    }

    public async Task<ServiceResult<WorkoutDetailDto>> GetWorkoutAsync(Guid workoutId, Guid userId)
    {
        var workout = await _db.Workouts
            .Include(w => w.WorkoutPlan)
            .Include(w => w.Sets)
                .ThenInclude(s => s.Exercise)
            .FirstOrDefaultAsync(w => w.Id == workoutId);

        if (workout == null)
            return ServiceResult<WorkoutDetailDto>.NotFound("Workout not found");
        if (workout.UserId != userId)
            return ServiceResult<WorkoutDetailDto>.Forbidden();

        return ServiceResult<WorkoutDetailDto>.Ok(_mapper.Map<WorkoutDetailDto>(workout));
    }

    public async Task<ServiceResult<WorkoutDetailDto>> LogWorkoutAsync(LogWorkoutDto dto, Guid userId)
    {
        if (dto.WorkoutPlanId.HasValue)
        {
            var plan = await _db.WorkoutPlans.FindAsync(dto.WorkoutPlanId.Value);
            if (plan == null)
                return ServiceResult<WorkoutDetailDto>.Fail("Workout plan not found");
        }

        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = dto.Date,
            WorkoutPlanId = dto.WorkoutPlanId,
            DurationMinutes = dto.DurationMinutes,
            CaloriesBurned = dto.CaloriesBurned,
            Notes = dto.Notes
        };

        foreach (var setDto in dto.Sets)
        {
            var exercise = await _db.Exercises.FindAsync(setDto.ExerciseId);
            if (exercise == null)
                return ServiceResult<WorkoutDetailDto>.Fail($"Exercise {setDto.ExerciseId} not found");

            var isPR = await CheckPersonalRecordAsync(setDto.ExerciseId, setDto.Weight, userId);

            workout.Sets.Add(new WorkoutSet
            {
                Id = Guid.NewGuid(),
                WorkoutId = workout.Id,
                ExerciseId = setDto.ExerciseId,
                SetNumber = setDto.SetNumber,
                Reps = setDto.Reps,
                Weight = setDto.Weight,
                IsPersonalRecord = isPR
            });
        }

        _db.Workouts.Add(workout);
        await _db.SaveChangesAsync();

        var created = await _db.Workouts
            .Include(w => w.WorkoutPlan)
            .Include(w => w.Sets)
                .ThenInclude(s => s.Exercise)
            .FirstAsync(w => w.Id == workout.Id);

        return ServiceResult<WorkoutDetailDto>.Created(_mapper.Map<WorkoutDetailDto>(created));
    }

    public async Task<ServiceResult<bool>> DeleteWorkoutAsync(Guid workoutId, Guid userId)
    {
        var workout = await _db.Workouts.FindAsync(workoutId);
        if (workout == null)
            return ServiceResult<bool>.NotFound("Workout not found");
        if (workout.UserId != userId)
            return ServiceResult<bool>.Forbidden();

        _db.Workouts.Remove(workout);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private async Task<bool> CheckPersonalRecordAsync(Guid exerciseId, decimal weight, Guid userId)
    {
        var maxPrevious = await _db.WorkoutSets
            .Include(s => s.Workout)
            .Where(s => s.ExerciseId == exerciseId && s.Workout.UserId == userId)
            .MaxAsync(s => (decimal?)s.Weight);

        return maxPrevious == null || weight > maxPrevious;
    }
}
