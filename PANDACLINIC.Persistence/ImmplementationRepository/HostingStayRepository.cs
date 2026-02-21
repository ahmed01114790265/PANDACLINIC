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
    public class HostingStayRepository : GenericRepository<HostingStay>, IHostingStayRepository
    {
        public HostingStayRepository(ClinicDbContext context) : base(context) { }

        // 1. Current Operations
        public async Task<IEnumerable<HostingStay>> GetCurrentStaysAsync()
        {
            return await _dbSet
                .Where(h => h.Status == HostingStayStatus.Ongoing)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetArrivalsByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(h => h.CheckInDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetDeparturesByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(h => h.CheckOutDate.HasValue && h.CheckOutDate.Value.Date == date.Date)
                .ToListAsync();
        }

        // 2. Animal History
        public async Task<IEnumerable<HostingStay>> GetHistoryByAnimalIdAsync(Guid animalId)
        {
            return await _dbSet
                .Where(h => h.AnimalId == animalId)
                .OrderByDescending(h => h.CheckInDate)
                .ToListAsync();
        }

        // 3. Status & Room Management
        public async Task<IEnumerable<HostingStay>> GetStaysByStatusAsync(HostingStayStatus status)
        {
            return await _dbSet.Where(h => h.Status == status).ToListAsync();
        }

        public async Task<HostingStay?> GetCurrentStayByRoomAsync(int roomNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(h => h.RoomNumber == roomNumber && h.Status == HostingStayStatus.Ongoing);
        }

        // 4. Availability Logic (Overlap Check)
        public async Task<bool> IsRoomAvailableAsync(int roomNumber, DateTime checkIn, DateTime checkOut)
        {
            // A room is NOT available if an existing stay is Scheduled or Ongoing 
            // AND the dates overlap.
            bool isOccupied = await _dbSet.AnyAsync(h =>
                h.RoomNumber == roomNumber &&
               (h.Status == HostingStayStatus.Scheduled || h.Status == HostingStayStatus.Ongoing) &&
               checkIn < (h.CheckOutDate ?? DateTime.MaxValue) &&
              checkOut > h.CheckInDate);
            return !isOccupied;
        }

        public async Task<IEnumerable<int>> GetAvailableRoomsAsync(DateTime start, DateTime end, int totalRoomsInClinic)
        {

            var occupiedRooms = await _dbSet
                .Where(h =>
                    // Only active 
                    (h.Status == HostingStayStatus.Scheduled || h.Status == HostingStayStatus.Ongoing) &&
                    // Overlap logic: (StartA < EndB) AND (EndA > StartB)
                    start < (h.CheckOutDate ?? DateTime.MaxValue) &&
                    end > h.CheckInDate)
                .Select(h => h.RoomNumber)
                .Distinct()
                .ToListAsync();

            var allRooms = Enumerable.Range(1, totalRoomsInClinic);

            return allRooms.Except(occupiedRooms).ToList();
        }
        // 5. Dashboard 
        public async Task<double> GetOccupancyRateAsync()
        {
            int totalRooms = 20;

            int occupiedCount = await _dbSet.CountAsync(h => h.Status == HostingStayStatus.Ongoing);

            if (totalRooms == 0) return 0;

            return (double)occupiedCount / totalRooms * 100;
        }

        public async Task<IEnumerable<HostingStay>> GetActiveStaysWithDetailsAsync()
        {

            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.Status == HostingStayStatus.Ongoing)
                .OrderBy(h => h.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetOverdueDeparturesAsync()
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h =>
                    h.Status == HostingStayStatus.Ongoing &&
                    h.CheckOutDate.HasValue &&
                    h.CheckOutDate.Value < now)
                .OrderBy(h => h.CheckOutDate)
                .ToListAsync();
        }
    }
}

