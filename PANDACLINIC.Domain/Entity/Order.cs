using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Domain.Comman.ValueObject;
using PANDACLINIC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
