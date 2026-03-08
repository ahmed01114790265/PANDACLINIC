using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Hosting
{
    public class HostingDetailDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int RoomNumber { get; set; }
        public HostingStayStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
