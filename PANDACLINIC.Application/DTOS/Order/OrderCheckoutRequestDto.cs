namespace PANDACLINIC.Application.DTOS.Order
{
    public class OrderCheckoutRequestDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = "CashOnDelivery";
        public List<OrderItemRequestDto> Items { get; set; } = new();
    }
}
