using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Where(o => o.UserId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbSet.Where(o => o.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Payments)
                .Where(o => o.Status == OrderStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Payments)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _dbSet
                .Include(o => o.User)
                .Include(o => o.Payments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(o =>
                    (o.User != null && o.User.UserName != null && o.User.UserName.Contains(searchTerm)) ||
                    o.Id.ToString().Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? start = null, DateTime? end = null)
        {
            var query = _dbSet.Where(o => o.Status == OrderStatus.Completed);

            if (start.HasValue)
                query = query.Where(o => o.CreatedAt >= start.Value);

            if (end.HasValue)
                query = query.Where(o => o.CreatedAt <= end.Value);

            return await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithPaymentIssuesAsync()
        {
            return await _dbSet
                .Include(o => o.Payments)
                .Where(o => o.Status != OrderStatus.Cancelled &&
                            o.Payments.Where(p => p.PaymentStatus == PaymentStatus.Completed)
                                      .Sum(p => (decimal?)p.Amount ?? 0) < o.TotalAmount)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetTopSellingItemsAsync(int count)
        {
            return await _context.Set<OrderItem>()
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { Item = g.First(), TotalQty = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.TotalQty)
                .Take(count)
                .Select(x => x.Item)
                .ToListAsync();
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet.CountAsync(o => o.CreatedAt.Date == today);
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(o => o.CreatedAt.Date == today && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<IEnumerable<Order>> GetDeletedOrdersAsync()
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Where(o => o.IsDeleted)
                .OrderByDescending(o => o.DeletedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdDeletedAsync(Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id && o.IsDeleted);
        }
    }
}
