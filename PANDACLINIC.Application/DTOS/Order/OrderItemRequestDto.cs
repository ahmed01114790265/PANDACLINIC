namespace PANDACLINIC.Application.DTOS.Order
{
    public class OrderItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
