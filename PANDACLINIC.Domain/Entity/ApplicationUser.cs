using Microsoft.AspNetCore.Identity;
using PANDACLINIC.Domain.Comman.SoftDelete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Entity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string fullName { get; set; } = string.Empty;
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
