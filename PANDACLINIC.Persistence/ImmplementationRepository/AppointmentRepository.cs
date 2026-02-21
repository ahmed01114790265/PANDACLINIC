using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext context) : base(context) { }

        // 1. Core Filtering
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            // We use .Date to compare only the calendar day, ignoring the specific time
            return await _dbSet
                .Where(a => a.AppointmentDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByAnimalIdAsync(Guid animalId)
        {
            return await _dbSet
                .Where(a => a.AnimalId == animalId)
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

        // 2. Status & Type Management
        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await _dbSet.Where(a => a.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByTypeAsync(AppointmentType type)
        {
            return await _dbSet.Where(a => a.TypeOfAppoinment == type).ToListAsync();
        }

        // 3. Dashboard 
        public async Task<IEnumerable<Appointment>> GetTodaysScheduleAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(a => a.Animal)
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }



        // 4. Capacity & Conflicts
        public async Task<int> GetCountByTimeSlotAsync(DateTime dateTime)
        {
            return await _dbSet.CountAsync(a => a.AppointmentDate == dateTime);
        }

        //public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId, DateTime? date = null)
        //{
        //    // This is a placeholder for future logic where you might link a Doctor/Staff ID
        //    // For now, it returns empty or can be adapted if you add a DoctorId to the entity
        //    return await _dbSet.Where(a => date == null || a.AppointmentDate.Date == date.Value.Date).ToListAsync();
        //}

        // 5. Reporting
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
                .Where(a => a.AnimalId == animalId && a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
    }
}
