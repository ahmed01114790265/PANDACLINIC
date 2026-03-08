using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class HostingStayRepository : GenericRepository<HostingStay>, IHostingStayRepository
    {
        public HostingStayRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<HostingStay>> GetCurrentStaysAsync()
        {
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.Status == HostingStayStatus.Ongoing)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetArrivalsByDateAsync(DateTime date)
        {
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.CheckInDate >= dayStart && h.CheckInDate < dayEnd)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetDeparturesByDateAsync(DateTime date)
        {
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.CheckOutDate.HasValue && h.CheckOutDate.Value >= dayStart && h.CheckOutDate.Value < dayEnd)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetHistoryByAnimalIdAsync(Guid animalId)
        {
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.AnimalId == animalId)
                .OrderByDescending(h => h.CheckInDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .OrderByDescending(h => h.CheckInDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostingStay>> GetByCreatorAsync(string creatorId)
        {
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.CreatedBy == creatorId)
                .OrderByDescending(h => h.CheckInDate)
                .ToListAsync();
        }

        public async Task<HostingStay?> GetHostingWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<HostingStay>> GetStaysByStatusAsync(HostingStayStatus status)
        {
            return await _dbSet
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.Status == status)
                .ToListAsync();
        }

        public async Task<HostingStay?> GetCurrentStayByRoomAsync(int roomNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(h => h.RoomNumber == roomNumber && h.Status == HostingStayStatus.Ongoing);
        }

        public async Task<bool> IsRoomAvailableAsync(int roomNumber, DateTime checkIn, DateTime checkOut)
        {
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
                    (h.Status == HostingStayStatus.Scheduled || h.Status == HostingStayStatus.Ongoing) &&
                    start < (h.CheckOutDate ?? DateTime.MaxValue) &&
                    end > h.CheckInDate)
                .Select(h => h.RoomNumber)
                .Distinct()
                .ToListAsync();

            var allRooms = Enumerable.Range(1, totalRoomsInClinic);
            return allRooms.Except(occupiedRooms).ToList();
        }

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
            var now = DateTime.Now;
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

        public async Task<IEnumerable<HostingStay>> GetDeletedHostingsAsync()
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(h => h.Animal)
                    .ThenInclude(a => a.User)
                .Where(h => h.IsDeleted)
                .OrderByDescending(h => h.DeletedAt)
                .ToListAsync();
        }

        public async Task<HostingStay?> GetByIdDeletedAsync(Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(h => h.Id == id && h.IsDeleted);
        }
    }
}
