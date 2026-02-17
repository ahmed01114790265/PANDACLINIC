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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id)
                   .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
            builder.Property(oi => oi.Quantity)
                   .IsRequired();
            builder.OwnsOne(oi => oi.Price, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("UnitPrice")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("Currency")
                     .HasMaxLength(3)
                     .HasDefaultValue("EGY");
            });


            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(oi => oi.Product)
                   .WithMany()
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
