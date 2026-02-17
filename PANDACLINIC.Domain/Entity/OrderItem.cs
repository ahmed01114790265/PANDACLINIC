using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Domain.Comman.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public Guid ProductId { get; set; }
        public virtual  Product Product { get; set; }
        public Money Price { get; set; }
    }
}
