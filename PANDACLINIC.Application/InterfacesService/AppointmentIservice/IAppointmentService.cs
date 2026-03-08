using PANDACLINIC.Application.BaseService;
using PANDACLINIC.Application.DTOS.Appointment;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.InterfacesService.AppointmentService
{
    public interface IAppointmentService : IBaseService<Appointment, AppointmentSummaryDto, AppointmentDetailDto, AppointmentRequestDto>
    {
        Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByAnimalIdAsync(Guid animalId);
        Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByCreatorAsync(Guid creatorId);
        Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByDateAsync(DateTime date);
        Task<Result<IEnumerable<AppointmentSummaryDto>>> GetByStatusAsync(AppointmentStatus status);
        Task<Result<IEnumerable<AppointmentSummaryDto>>> GetTodaysScheduleAsync();
        Task<Result<AppointmentDetailDto>> GetDetailsAsync(Guid appointmentId);
        Task<Result> UpdateAsync(AppointmentUpdateDto dto);
        Task<Result> UpdateStatusAsync(Guid appointmentId, AppointmentStatus status);
        Task<Result<IEnumerable<AppointmentSummaryDto>>> GetDeletedAppointmentsAsync();
        Task<Result> RestoreAsync(Guid id);
    }
}
