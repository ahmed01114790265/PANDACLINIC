using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Hosting
{
    public class HostingUpdateDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int RoomNumber { get; set; }
        public HostingStayStatus Status { get; set; }
    }
}
