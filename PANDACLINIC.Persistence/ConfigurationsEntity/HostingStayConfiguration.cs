using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PANDACLINIC.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.ConfigurationsEntity
{
    public class HostingStayConfiguration : IEntityTypeConfiguration<HostingStay>
    {
        public void Configure(EntityTypeBuilder<HostingStay> builder)
        {
            builder.ToTable("HostingStays");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id)
                   .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
            builder.Property(h => h.CheckInDate)
                   .IsRequired();
            builder.Property(h => h.CheckOutDate)
                   .IsRequired(false);
            builder.Property(h => h.RoomNumber)
                   .IsRequired();
            builder.Property(h => h.Status)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();
            builder.HasOne(h => h.Animal)
                   .WithMany(a => a.HostingHistory)
                   .HasForeignKey(h => h.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => h.CheckOutDate);
            builder.HasIndex(h => h.RoomNumber);
        }
    }
}
