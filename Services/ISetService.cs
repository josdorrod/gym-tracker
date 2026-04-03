using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymTracker.DTO;

namespace GymTracker.Services;

public interface ISetService
{
    public Task<List<SetDetailListDTO>> GetAllSetsAsync(int exerciseId);
    public Task<int> CreateSetAsync(int exerciseId, SetInputDTO setDto);
    public Task<bool> DeleteSetAsync(int setId);
    public Task<int> GetTodaySetCount(int exerciseId);
}