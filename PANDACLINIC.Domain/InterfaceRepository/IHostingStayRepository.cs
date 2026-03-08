using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IHostingStayRepository : IGenericRepository<HostingStay>
    {
        Task<IEnumerable<HostingStay>> GetCurrentStaysAsync();
        Task<IEnumerable<HostingStay>> GetArrivalsByDateAsync(DateTime date);
        Task<IEnumerable<HostingStay>> GetDeparturesByDateAsync(DateTime date);

        Task<IEnumerable<HostingStay>> GetHistoryByAnimalIdAsync(Guid animalId);
        Task<IEnumerable<HostingStay>> GetAllWithDetailsAsync();
        Task<IEnumerable<HostingStay>> GetByCreatorAsync(string creatorId);
        Task<HostingStay?> GetHostingWithDetailsAsync(Guid id);

        Task<IEnumerable<HostingStay>> GetStaysByStatusAsync(HostingStayStatus status);
        Task<HostingStay?> GetCurrentStayByRoomAsync(int roomNumber);

        Task<bool> IsRoomAvailableAsync(int roomNumber, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<int>> GetAvailableRoomsAsync(DateTime start, DateTime end, int totalRoomsInClinic);

        Task<double> GetOccupancyRateAsync();
        Task<IEnumerable<HostingStay>> GetActiveStaysWithDetailsAsync();
        Task<IEnumerable<HostingStay>> GetOverdueDeparturesAsync();

        Task<IEnumerable<HostingStay>> GetDeletedHostingsAsync();
        Task<HostingStay?> GetByIdDeletedAsync(Guid id);
    }
}

