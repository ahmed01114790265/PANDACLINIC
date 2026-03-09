using System.Text.Json;
using PANDACLINIC.Application.DTOS.Order;
using PANDACLINIC.Application.InterfacesService.OrderService;
using PANDACLINIC.Domain.Comman.ValueObject;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.ImmplementationServices.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<OrderDetailDto>> CreateAsync(Guid userId, OrderCheckoutRequestDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return Result<OrderDetailDto>.Failure("Cart is empty.");

            if (string.IsNullOrWhiteSpace(dto.ClientName) || string.IsNullOrWhiteSpace(dto.ClientPhone) || string.IsNullOrWhiteSpace(dto.ClientAddress))
                return Result<OrderDetailDto>.Failure("Client name, phone and address are required.");

            var paymentMethod = NormalizePaymentMethod(dto.PaymentMethod);
            if (paymentMethod == null)
                return Result<OrderDetailDto>.Failure("Unsupported payment method.");

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToHashSet();
            var products = (await _uow.Products.GetAllAsync())
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToDictionary(p => p.Id, p => p);

            if (products.Count != productIds.Count)
                return Result<OrderDetailDto>.Failure("One or more products are unavailable.");

            decimal total = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    return Result<OrderDetailDto>.Failure("Quantity must be greater than zero.");

                var product = products[item.ProductId];
                if (product.Stock < item.Quantity)
                    return Result<OrderDetailDto>.Failure($"Insufficient stock for {product.Name}.");

                var priceAmount = ApplyDiscount(product.Price.Amount, product.DiscountPercentage);
                total += priceAmount * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = new Money(priceAmount, product.Price.Currency)
                });

                product.Stock -= item.Quantity;
                _uow.Products.Update(product);
            }

            var clientMeta = new ClientMeta
            {
                ClientName = dto.ClientName.Trim(),
                ClientPhone = dto.ClientPhone.Trim(),
                ClientAddress = dto.ClientAddress.Trim(),
                Notes = dto.Notes?.Trim(),
                IsClientConfirmed = true
            };

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                CreatedBy = JsonSerializer.Serialize(clientMeta),
                OrderItems = orderItems,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        PaymentMethod = paymentMethod,
                        PaymentStatus = PaymentStatus.New,
                        Amount = total
                    }
                }
            };

            await _uow.Orders.AddAsync(order);
            await _uow.CompleteAsync();

            var created = await _uow.Orders.GetOrderWithDetailsAsync(order.Id);
            if (created == null)
                return Result<OrderDetailDto>.Failure("Failed to load created order.");

            return Result<OrderDetailDto>.Success(MapToDetail(created), "Order created successfully.");
        }

        public async Task<Result<IEnumerable<OrderSummaryDto>>> GetByCustomerAsync(Guid userId)
        {
            var orders = await _uow.Orders.GetOrdersByCustomerIdAsync(userId);
            return Result<IEnumerable<OrderSummaryDto>>.Success(orders.Select(MapToSummary));
        }

        public async Task<Result<IEnumerable<OrderSummaryDto>>> GetAllAsync()
        {
            var orders = await _uow.Orders.GetAllWithDetailsAsync();
            return Result<IEnumerable<OrderSummaryDto>>.Success(orders.Select(MapToSummary));
        }

        public async Task<Result<OrderDetailDto>> GetDetailsAsync(Guid orderId)
        {
            var order = await _uow.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return Result<OrderDetailDto>.Failure("Order not found.");

            return Result<OrderDetailDto>.Success(MapToDetail(order));
        }

        public async Task<Result> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _uow.Orders.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure("Order not found.");

            order.Status = status;
            _uow.Orders.Update(order);
            await _uow.CompleteAsync();
            return Result.Success("Order status updated successfully.");
        }

        public async Task<Result> UpdatePaymentStatusAsync(Guid orderId, PaymentStatus paymentStatus)
        {
            var order = await _uow.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return Result.Failure("Order not found.");

            var payment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            if (payment == null)
                return Result.Failure("Payment record not found for this order.");

            payment.PaymentStatus = paymentStatus;
            if (paymentStatus == PaymentStatus.Completed)
                order.Status = OrderStatus.Completed;

            _uow.Orders.Update(order);
            await _uow.CompleteAsync();
            return Result.Success("Payment status updated successfully.");
        }

        public async Task<Result> DeleteAsync(Guid orderId)
        {
            var order = await _uow.Orders.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure("Order not found.");

            _uow.Orders.Delete(order);
            await _uow.CompleteAsync();
            return Result.Success("Order moved to archive successfully.");
        }

        public async Task<Result<IEnumerable<OrderSummaryDto>>> GetDeletedOrdersAsync()
        {
            var orders = await _uow.Orders.GetDeletedOrdersAsync();
            return Result<IEnumerable<OrderSummaryDto>>.Success(orders.Select(MapToSummary));
        }

        public async Task<Result> RestoreAsync(Guid id)
        {
            var order = await _uow.Orders.GetByIdDeletedAsync(id);
            if (order == null)
                return Result.Failure("Order not found in archive.");

            order.IsDeleted = false;
            order.DeletedAt = null;
            await _uow.CompleteAsync();
            return Result.Success("Order restored successfully.");
        }

        private static decimal ApplyDiscount(decimal amount, int? discount)
        {
            if (!discount.HasValue || discount.Value <= 0) return amount;
            if (discount.Value >= 100) return 0;
            return amount - (amount * discount.Value / 100m);
        }

        private static string? NormalizePaymentMethod(string? paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod)) return null;

            return paymentMethod.Trim().ToLowerInvariant() switch
            {
                "vodafonecash" => "VodafoneCash",
                "cashondelivery" => "CashOnDelivery",
                _ => null
            };
        }

        private static OrderSummaryDto MapToSummary(Order order)
        {
            var meta = ParseMeta(order.CreatedBy);
            var payment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

            return new OrderSummaryDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                ClientName = meta.ClientName ?? order.User?.fullName ?? string.Empty,
                ClientPhone = meta.ClientPhone ?? order.User?.PhoneNumber ?? string.Empty,
                ClientAddress = meta.ClientAddress ?? string.Empty,
                IsClientConfirmed = meta.IsClientConfirmed,
                PaymentMethod = payment?.PaymentMethod ?? string.Empty,
                PaymentStatus = payment?.PaymentStatus ?? PaymentStatus.New,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ItemsCount = order.OrderItems.Count
            };
        }

        private static OrderDetailDto MapToDetail(Order order)
        {
            var meta = ParseMeta(order.CreatedBy);
            var payment = order.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

            return new OrderDetailDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                UserId = order.UserId,
                ClientName = meta.ClientName ?? order.User?.fullName ?? string.Empty,
                ClientPhone = meta.ClientPhone ?? order.User?.PhoneNumber ?? string.Empty,
                ClientAddress = meta.ClientAddress ?? string.Empty,
                IsClientConfirmed = meta.IsClientConfirmed,
                Notes = meta.Notes,
                PaymentMethod = payment?.PaymentMethod ?? string.Empty,
                PaymentStatus = payment?.PaymentStatus ?? PaymentStatus.New,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemSummaryDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price.Amount,
                    LineTotal = i.Quantity * i.Price.Amount
                }).ToList()
            };
        }

        private static ClientMeta ParseMeta(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new ClientMeta();

            try
            {
                return JsonSerializer.Deserialize<ClientMeta>(value) ?? new ClientMeta();
            }
            catch
            {
                return new ClientMeta();
            }
        }

        private sealed class ClientMeta
        {
            public string? ClientName { get; set; }
            public string? ClientPhone { get; set; }
            public string? ClientAddress { get; set; }
            public string? Notes { get; set; }
            public bool IsClientConfirmed { get; set; }
        }
    }
}
