using PANDACLINIC.Application.DTOS.Order;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.InterfacesService.OrderService
{
    public interface IOrderService
    {
        Task<Result<OrderDetailDto>> CreateAsync(Guid userId, OrderCheckoutRequestDto dto);
        Task<Result<IEnumerable<OrderSummaryDto>>> GetByCustomerAsync(Guid userId);
        Task<Result<IEnumerable<OrderSummaryDto>>> GetAllAsync();
        Task<Result<OrderDetailDto>> GetDetailsAsync(Guid orderId);
        Task<Result> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<Result> UpdatePaymentStatusAsync(Guid orderId, PaymentStatus paymentStatus);
        Task<Result> DeleteAsync(Guid orderId);
        Task<Result<IEnumerable<OrderSummaryDto>>> GetDeletedOrdersAsync();
        Task<Result> RestoreAsync(Guid id);
    }
}
