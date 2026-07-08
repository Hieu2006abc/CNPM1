using HeartCareAI.Models;

namespace HeartCareAI.Repositories;

public interface IPredictionRepository
{
    Task<IReadOnlyList<Prediction>> GetAllAsync();

    Task<Prediction?> GetByIdAsync(Guid id);

    Task AddAsync(Prediction prediction);
}
