using PANDACLINIC.Application.BaseService;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Shared.ResultModel;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Application.InterfacesService.AnimalService
{
    public interface IAnimalService : IBaseService<Animal, AnimalSummaryDto, AnimalDetailDto, AnimalRequestDto>
    {
        Task<Result<IEnumerable<AnimalSummaryDto>>> GetByOwnerAsync(Guid ownerId);
        Task<Result<AnimalDetailDto>> GetDetailsAsync(Guid animalId);
        Task<Result<PagedResponseDto<AnimalSummaryDto>>> GetPagedAsync(int page, int size, string? search);
        Task<Result<IEnumerable<AnimalSummaryDto>>> GetCurrentlyHostedAsync();
        Task<Result<bool>> CheckNameAvailabilityAsync(string name, Guid ownerId);
            Task<Result<IEnumerable<AnimalSummaryDto>>> GetDeletedAnimalsAsync();
        Task<Result> RestoreAsync(Guid id);
    }
}
