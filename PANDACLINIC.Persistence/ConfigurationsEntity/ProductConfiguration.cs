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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.SequentialGuidValueGenerator>();
            builder.Property(p => p.Name)
                   .HasMaxLength(150)
                   .IsRequired();
            builder.Property(p => p.Description)
                   .HasMaxLength(1000);
            builder.Property(p => p.Taste)
                   .HasMaxLength(50);
            builder.Property(p => p.ImageUrl)
                   .HasDefaultValue("default-product.png");
            builder.OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("Price")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("Currency")
                     .HasMaxLength(3)
                     .HasDefaultValue("EGY");
            });
            builder.Property(p => p.Weight)
                   .HasColumnType("decimal(18,3)");
            builder.Property(p => p.Stock)
                   .IsRequired();
            builder.Property(p => p.Type)
                   .HasConversion<string>()
                   .HasMaxLength(50);
            builder.Property(p => p.IsActive)
                   .HasDefaultValue(true);

            builder.HasMany(p => p.OrderItems)
                   .WithOne(oi => oi.Product)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.IsActive);
        }
    }
}
