using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.DTOS.Animal
{
    public class AnimalRequestDto
    {
        public string Name { get; set; } = string.Empty;
        //public string Species { get; set; } = string.Empty;
        //public string Breed { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Guid UserId { get; set; }
        public string? Imgageurl { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
