using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Order
{
    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public bool IsClientConfirmed { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
    }
}
