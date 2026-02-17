using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    public class Payment : BaseEntity
    {
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
