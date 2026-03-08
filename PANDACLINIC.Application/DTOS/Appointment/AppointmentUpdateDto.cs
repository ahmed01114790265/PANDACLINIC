using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Appointment
{
    public class AppointmentUpdateDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public AppointmentType TypeOfAppoinment { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
