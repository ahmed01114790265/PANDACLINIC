using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IHostingStayRepository : IGenericRepository<HostingStay>
    {
        // Animals currently physically in the clinic
        Task<IEnumerable<HostingStay>> GetCurrentStaysAsync();

        // Arriving today
        Task<IEnumerable<HostingStay>> GetArrivalsByDateAsync(DateTime date);

        // Scheduled to leave today
        Task<IEnumerable<HostingStay>> GetDeparturesByDateAsync(DateTime date);

        // --- Animal History ---
        Task<IEnumerable<HostingStay>> GetHistoryByAnimalIdAsync(Guid animalId);

        // --- Status & Room Management ---
        Task<IEnumerable<HostingStay>> GetStaysByStatusAsync(HostingStayStatus status);

        // Find who is in a specific room/cage
        Task<HostingStay?> GetCurrentStayByRoomAsync(int roomNumber);

        
        // Check if a specific room is free during a date range
        Task<bool> IsRoomAvailableAsync(int roomNumber, DateTime checkIn, DateTime checkOut);

        // Find all empty rooms for a specific date range
        Task<IEnumerable<int>> GetAvailableRoomsAsync(DateTime start, DateTime end, int totalRoomsInClinic);

        // --- Dashboard 
        // Occupancy percentage (e.g., "Clinic is 80% full")
        Task<double> GetOccupancyRateAsync();

        // Get details including Animal and Owner for the "Boarding List" view
        Task<IEnumerable<HostingStay>> GetActiveStaysWithDetailsAsync();

        // Animals that missed their checkout time/date
        Task<IEnumerable<HostingStay>> GetOverdueDeparturesAsync();
    }
}
