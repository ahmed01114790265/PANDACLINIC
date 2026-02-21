using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class AnimalRepository : GenericRepository<Animal>, IAnimalRepository
    {
        public AnimalRepository(ClinicDbContext context) : base(context) { }

        // 1. Basic Lookups
        public async Task<IEnumerable<Animal>> GetAnimalsByOwnerIdAsync(Guid ownerId)
        {
            return await _dbSet
                .Where(a => a.UserId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Animal>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(a => a.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<Animal?> GetAnimalWithDetailsAsync(Guid animalId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.Appointments)
                .Include(a => a.HostingHistory)
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        // 2. Advanced Filtering & Pagination
        public async Task<(IEnumerable<Animal> Items, int TotalCount)> GetPagedAnimalsAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // 3. Relationship & History Tracking


        public async Task<IEnumerable<Animal>> GetAnimalsCurrentlyInHostingAsync()
        {
            return await _dbSet
                .Where(a => a.HostingHistory.Any(h => h.CheckOutDate == null))
                .ToListAsync();
        }


        // 4. Reporting & Statistics
        public async Task<int> GetTotalCountByOwnerIdAsync(Guid ownerId)
        {
            return await _dbSet.CountAsync(a => a.UserId == ownerId);
        }

        public async Task<bool> IsNameExistsForOwnerAsync(string name, Guid ownerId)
        {
            return await _dbSet.AnyAsync(a => a.Name.ToLower() == name.ToLower() && a.UserId == ownerId);
        }

        // 5. Soft Delete Recovery
        public async Task<IEnumerable<Animal>> GetDeletedAnimalsAsync()
        {
            // IgnoreQueryFilters allows us to see the records where IsDeleted = true
            return await _dbSet
                .IgnoreQueryFilters()
                .Where(a => a.IsDeleted)
                .ToListAsync();
        }
    }
}
