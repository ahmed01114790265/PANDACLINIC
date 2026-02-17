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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
            builder.Property(p => p.PaymentStatus)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();
            builder.Property(p => p.PaymentMethod)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(p => p.Order)
                   .WithMany(o => o.Payments)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.PaymentStatus);
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
