using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using PANDACLINIC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ClinicDbContext context) : base(context) { }

        // 1.  Filters
        public async Task<IEnumerable<Product>> GetInStockProductsAsync()
        {
            return await _dbSet.Where(p => p.Stock > 0 && p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByTypeAsync(ProductType type)
        {
            return await _dbSet.Where(p => p.Type == type && p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetDiscountedProductsAsync()
        {
            // Assuming you have a DiscountPrice or IsOnSale property
            return await _dbSet.Where(p => p.DiscountPercentage > 0 && p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByTasteAsync(string taste)
        {
            return await _dbSet
                .Where(p => p.Taste != null && p.Taste.Contains(taste) && p.IsActive)
                .ToListAsync();
        }

        // 2. Search & Pagination
        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Where(p => (p.Name.Contains(searchTerm) || p.Taste.Contains(searchTerm)) && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, ProductType? type = null, string? searchTerm = null)
        {
            var query = _dbSet.AsQueryable();

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(p => p.Name.Contains(searchTerm));

            return await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // 3. Inventory Management
        public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync()
        {
            return await _dbSet.Where(p => p.Stock == 0 && p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockAlertsAsync(int threshold)
        {
            return await _dbSet.Where(p => p.Stock <= threshold && p.IsActive).ToListAsync();
        }

        public async Task UpdateProductStatusAsync(Guid productId, bool isActive)
        {
            var product = await _dbSet.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = isActive;
            }
        }

        // 4. Analytics
        public async Task<IEnumerable<Product>> GetBestSellersAsync(int topCount)
        {
            // Joining with OrderItems to find most sold products
            return await _context.Set<OrderItem>()
                .GroupBy(oi => oi.ProductId)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Select(g => g.First().Product)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            // Sum of (Price * Quantity) for all active products
            return await _dbSet
                .Where(p => p.IsActive)
                .SumAsync(p => p.Price.Amount * p.Stock);
        }

        public async Task<bool> IsProductNameUniqueAsync(string name)
        {
            return !await _dbSet.AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }
    }
}