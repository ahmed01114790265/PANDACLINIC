using PANDACLINIC.Application.BaseService;
using PANDACLINIC.Application.DTOS.Hosting;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.InterfacesService.HostingService
{
    public interface IHostingService : IBaseService<HostingStay, HostingSummaryDto, HostingDetailDto, HostingRequestDto>
    {
        Task<Result<IEnumerable<HostingSummaryDto>>> GetByAnimalIdAsync(Guid animalId);
        Task<Result<IEnumerable<HostingSummaryDto>>> GetByCreatorAsync(Guid creatorId);
        Task<Result<IEnumerable<HostingSummaryDto>>> GetByDateAsync(DateTime date);
        Task<Result<IEnumerable<HostingSummaryDto>>> GetByStatusAsync(HostingStayStatus status);
        Task<Result<HostingDetailDto>> GetDetailsAsync(Guid hostingId);
        Task<Result> UpdateAsync(HostingUpdateDto dto);
        Task<Result> UpdateStatusAsync(Guid hostingId, HostingStayStatus status);
        Task<Result<IEnumerable<HostingSummaryDto>>> GetDeletedHostingsAsync();
        Task<Result> RestoreAsync(Guid id);
    }
}
