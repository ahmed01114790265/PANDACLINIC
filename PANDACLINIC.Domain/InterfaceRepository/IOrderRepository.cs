using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<Order?> GetOrderWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();

        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetPendingOrdersAsync();

        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null);

        Task<decimal> GetTotalRevenueAsync(DateTime? start = null, DateTime? end = null);
        Task<IEnumerable<Order>> GetOrdersWithPaymentIssuesAsync();

        Task<IEnumerable<OrderItem>> GetTopSellingItemsAsync(int count);

        Task<int> GetTodayOrdersCountAsync();
        Task<decimal> GetTodayRevenueAsync();

        Task<IEnumerable<Order>> GetDeletedOrdersAsync();
        Task<Order?> GetByIdDeletedAsync(Guid id);
    }
}
