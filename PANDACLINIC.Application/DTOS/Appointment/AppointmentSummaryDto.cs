using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Appointment
{
    public class AppointmentSummaryDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public AppointmentType TypeOfAppoinment { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
