using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Domain.Comman.ValueObject;
using PANDACLINIC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Money Price { get; set; } = new Money(0, "EGP");
        public decimal Weight { get; set; }
        public string Taste { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "default-product.png";
        public int? DiscountPercentage { get; set; } 
        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        public ProductType Type { get; set; } 
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}

