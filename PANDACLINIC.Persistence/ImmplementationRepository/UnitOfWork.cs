using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Persistence.ImmplementationRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _context;
        public IAnimalRepository Animals { get; }
        public IAppointmentRepository Appointments { get; }
        public IHostingStayRepository HostingStays { get; }
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(ClinicDbContext context)
        {
            _context = context;

            // Passing the same _context instance to all repos

            Animals = new AnimalRepository(_context);
            Appointments = new AppointmentRepository(_context);
            HostingStays = new HostingStayRepository(_context);
            Orders = new OrderRepository(_context);
            Products = new ProductRepository(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
