using PANDACLINIC.Domain.Comman.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    public class Animal : BaseEntity
    {
        public string Name { get; private set; }
        public Guid UserId { get; set; }
        public string? Imgageurl { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<HostingStay> HostingHistory { get; set; } = new List<HostingStay>();

    }
 } 

