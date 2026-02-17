using PANDACLINIC.Domain.Comman.SoftDelete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Comman.BaseEntity
{
    public abstract class BaseEntity : ISoftDelete
    {
        public Guid Id { get; protected set; }= Guid.NewGuid();
       
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string? LastUpdatedBy { get; set; }

       
    }
}
