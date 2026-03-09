using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetByAnimalIdAsync(Guid animalId);
        Task<IEnumerable<Appointment>> GetByCreatorAsync(string creatorId);
        Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id);

        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetAppointmentsByTypeAsync(AppointmentType type);

        Task<IEnumerable<Appointment>> GetTodaysScheduleAsync();

        Task<int> GetCountByTimeSlotAsync(DateTime dateTime);

        Task<int> GetCountByTypeAndDateRangeAsync(AppointmentType type, DateTime start, DateTime end);
        Task<IEnumerable<Appointment>> GetMedicalHistoryAsync(Guid animalId);

        Task<IEnumerable<Appointment>> GetDeletedAppointmentsAsync();
        Task<Appointment?> GetByIdDeletedAsync(Guid id);
    }
}
