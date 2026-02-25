using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IAnimalRepository : IGenericRepository<Animal>
    {
        Task<IEnumerable<Animal>> GetAnimalsByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<Animal>> SearchByNameAsync(string name);
        Task<Animal?> GetAnimalWithDetailsAsync(Guid animalId); // Includes User, Appointments, Hosting

        // --- Advanced Filtering (Dashboard/Admin) ---
        // Returns animals with pagination to keep the Dashboard fast
        Task<(IEnumerable<Animal> Items, int TotalCount)> GetPagedAnimalsAsync(int pageNumber, int pageSize, string? searchTerm = null);

        // --- Relationship & History Tracking ---
        // Useful for the "Patient File" view in the clinic
        Task<IEnumerable<Animal>> GetAnimalsCurrentlyInHostingAsync();

        // --- Reporting & Statistics ---
        Task<int> GetTotalCountByOwnerIdAsync(Guid ownerId);
        Task<bool> IsNameExistsForOwnerAsync(string name, Guid ownerId);

        // --- Soft Delete / Activation (If applicable) ---
        Task<IEnumerable<Animal>> GetDeletedAnimalsAsync();
        Task<Animal?> GetByIdDeletedAsync(Guid id);// If using a 'IsDeleted' flag
    }
}
