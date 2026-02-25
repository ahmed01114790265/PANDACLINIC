using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.DTOS.Animal
{
    public class AnimalDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //public string Species { get; set; } = string.Empty;
        //public string Breed { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
         public string? Imgageurl { get; set; }

        // Flattened data via AutoMapper
        public string OwnerName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
        public bool IsHostedNow { get; set; }
    }
}
