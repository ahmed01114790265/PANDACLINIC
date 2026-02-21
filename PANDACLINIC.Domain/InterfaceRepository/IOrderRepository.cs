using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // --- Basic 
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId);

        // Includes OrderItems, Products, and Payments
        Task<Order?> GetOrderWithDetailsAsync(Guid orderId);

        // --- Status & Sales Management ---
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);

        // Orders waiting for payment or processing
        Task<IEnumerable<Order>> GetPendingOrdersAsync();

        // ---  (Dashboard) ---
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Paged results for Admin "All Orders" view
        Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedOrdersAsync(int pageNumber, int pageSize, string? searchTerm = null);

        // Calculates total revenue within a period
        Task<decimal> GetTotalRevenueAsync(DateTime? start = null, DateTime? end = null);

        // Identifies orders that have partial or failed payments
        Task<IEnumerable<Order>> GetOrdersWithPaymentIssuesAsync();

        // --- Order Item Analysis ---
        // Useful for finding "Best Selling Products"
        Task<IEnumerable<OrderItem>> GetTopSellingItemsAsync(int count);

        // For Dashboard stats cards
        Task<int> GetTodayOrdersCountAsync();
        Task<decimal> GetTodayRevenueAsync();
    }
}
