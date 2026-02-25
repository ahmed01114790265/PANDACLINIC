using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.ConfigurationsEntity
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                   .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
            builder.Property(a => a.TypeOfAppoinment)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .HasDefaultValue(AppointmentStatus.Scheduled);


            builder.HasOne(a => a.Animal)
                   .WithMany(an => an.Appointments)
                   .HasForeignKey(a => a.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);

            
            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => a.CreatedAt);
        }
    }
}
