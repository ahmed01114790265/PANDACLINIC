using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    public class HostingStay : BaseEntity
    {
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; } 
        public HostingStayStatus Status { get; set; }   
        public int RoomNumber { get; set; }

        public Guid AnimalId { get; set; }
        public virtual Animal Animal { get; set; } = null!;
    }
}
