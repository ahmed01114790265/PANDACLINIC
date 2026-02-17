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
    public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
    {
        public void Configure(EntityTypeBuilder<Animal> builder)
        {

            builder.ToTable("Animals");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                   .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.IsDeleted).HasDefaultValue(false);


            builder.HasOne(a => a.User)
                   .WithMany(u => u.Animals)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(a => a.Appointments)
                   .WithOne(ap => ap.Animal)
                   .HasForeignKey(ap => ap.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(a => a.HostingHistory)
                   .WithOne(h => h.Animal)
                   .HasForeignKey(h => h.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
