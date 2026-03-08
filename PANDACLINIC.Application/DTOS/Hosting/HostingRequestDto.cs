using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Hosting
{
    public class HostingRequestDto
    {
        public Guid AnimalId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int RoomNumber { get; set; }
        public HostingStayStatus Status { get; set; } = HostingStayStatus.Scheduled;
        public string? CreatedBy { get; set; }

        // Web reservation confirmation fields
        public string? ClientFullName { get; set; }
        public string? ClientPhone { get; set; }
        public bool IsClientConfirmed { get; set; }
    }
}
