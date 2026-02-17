using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PANDACLINIC.Domain.Comman.BaseEntity;
using PANDACLINIC.Domain.Comman.SoftDelete;
using PANDACLINIC.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.Context
{
    public class ClinicDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<HostingStay> HostingStays => Set<HostingStay>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Property("CreatedAt").IsRequired();
                    builder.Entity(entityType.ClrType).Property("CreatedBy").HasMaxLength(256);
                }

                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Property("IsDeleted").HasDefaultValue(false);
                    builder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastUpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        if (entry.Entity is ISoftDelete softDelete)
                        {
                            entry.State = EntityState.Modified;
                            softDelete.IsDeleted = true;

                        }
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(Type type)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var falseConstant = System.Linq.Expressions.Expression.Constant(false);
            var body = System.Linq.Expressions.Expression.Equal(property, falseConstant);
            return System.Linq.Expressions.Expression.Lambda(body, parameter);
        }
    }
}
