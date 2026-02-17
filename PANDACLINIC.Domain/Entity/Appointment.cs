using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    public class Appointment : BaseEntity
    {
        public AppointmentType TypeOfAppoinment { get; set; }
        public AppointmentStatus Status { get; set; }
        public Guid AnimalId { get; set; }
        public virtual Animal Animal { get; set; } = null!;
    }
}
