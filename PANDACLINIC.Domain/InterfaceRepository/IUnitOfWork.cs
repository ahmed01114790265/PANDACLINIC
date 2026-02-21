using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IAnimalRepository Animals { get; }
        IAppointmentRepository Appointments { get; }
        IHostingStayRepository HostingStays { get; }
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        
        // Commits all changes tracked by the DbContext to the database
        Task<int> CompleteAsync();
    }
}
