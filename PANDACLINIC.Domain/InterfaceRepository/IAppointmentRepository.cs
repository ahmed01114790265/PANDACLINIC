using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetByAnimalIdAsync(Guid animalId);
        Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id); // Includes Animal and Owner

        // --- Status & Type Management ---
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetAppointmentsByTypeAsync(AppointmentType type);

        // --- Dashboard & Operations ---
        // Get all appointments for today with Animal and User data included
        Task<IEnumerable<Appointment>> GetTodaysScheduleAsync();

        // --- Capacity & Conflicts ---
        // Check how many appointments of a certain type exist at a specific time
        Task<int> GetCountByTimeSlotAsync(DateTime dateTime);

        // Useful if you decide to link Doctors to appointments later
       // Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId, DateTime? date = null);

        // --- Reporting & Statistics ---
        // For Dashboard charts (e.g., Number of surgeries this month)
        Task<int> GetCountByTypeAndDateRangeAsync(AppointmentType type, DateTime start, DateTime end);

        // Get the history of a specific animal's medical visits
        Task<IEnumerable<Appointment>> GetMedicalHistoryAsync(Guid animalId);
    }
}
