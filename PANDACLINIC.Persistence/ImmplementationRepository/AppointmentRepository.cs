using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);

            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.AppointmentDate >= dayStart && a.AppointmentDate < dayEnd)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByAnimalIdAsync(Guid animalId)
        {
            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.AnimalId == animalId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByCreatorAsync(string creatorId)
        {
            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.CreatedBy == creatorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(a => a.Animal)
                    .ThenInclude(an => an.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByTypeAsync(AppointmentType type)
        {
            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.TypeOfAppoinment == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodaysScheduleAsync()
        {
            var todayStart = DateTime.Now.Date;
            var todayEnd = todayStart.AddDays(1);

            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.AppointmentDate >= todayStart && a.AppointmentDate < todayEnd)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<int> GetCountByTimeSlotAsync(DateTime dateTime)
        {
            var slotStart = new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                0);
            var slotEnd = slotStart.AddMinutes(1);

            return await _dbSet.CountAsync(a => a.AppointmentDate >= slotStart && a.AppointmentDate < slotEnd);
        }

        public async Task<int> GetCountByTypeAndDateRangeAsync(AppointmentType type, DateTime start, DateTime end)
        {
            return await _dbSet.CountAsync(a =>
                a.TypeOfAppoinment == type &&
                a.AppointmentDate >= start &&
                a.AppointmentDate <= end);
        }

        public async Task<IEnumerable<Appointment>> GetMedicalHistoryAsync(Guid animalId)
        {
            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.AnimalId == animalId && a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDeletedAppointmentsAsync()
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(a => a.Animal)
                .Where(a => a.IsDeleted)
                .OrderByDescending(a => a.DeletedAt)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdDeletedAsync(Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted);
        }
    }
}
