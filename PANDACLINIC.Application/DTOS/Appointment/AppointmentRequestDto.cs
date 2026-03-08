using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Appointment
{
    public class AppointmentRequestDto
    {
        public Guid AnimalId { get; set; }
        public AppointmentType TypeOfAppoinment { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
