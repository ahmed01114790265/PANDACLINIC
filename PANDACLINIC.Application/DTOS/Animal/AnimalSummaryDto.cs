using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.DTOS.Animal
{
    public class AnimalSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Imgageurl { get; set; }
        //public string Species { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
