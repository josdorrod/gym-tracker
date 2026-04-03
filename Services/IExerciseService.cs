using GymTracker.DTO;

namespace GymTracker.Services;
public interface IExerciseService
{
    public Task<ExerciseCreateDTO?> GetExerciseByIdAsync(int id);
    public Task<List<ExerciseListDTO>> GetAllExercisesAsync();
    public Task<int> CreateExerciseAsync(ExerciseCreateDTO exerciseDto);
    public Task<bool> UpdateExerciseAsync(int id, ExerciseCreateDTO exerciseDto);
}