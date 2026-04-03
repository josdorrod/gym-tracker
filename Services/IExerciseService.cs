using GymTracker.DTO;

namespace GymTracker.Services;
public interface IExerciseService
{
    public Task<ExerciseInputDTO?> GetExerciseByIdAsync(int id);
    public Task<List<ExerciseListDTO>> GetAllExercisesAsync();
    public Task<int> CreateExerciseAsync(ExerciseInputDTO exerciseDto);
    public Task<bool> UpdateExerciseAsync(int id, ExerciseInputDTO exerciseDto);
}