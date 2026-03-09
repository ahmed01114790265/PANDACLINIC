using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Order
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public bool IsClientConfirmed { get; set; }
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemSummaryDto> Items { get; set; } = new();
        public string? WhatsAppUrl { get; set; }
    }
}
