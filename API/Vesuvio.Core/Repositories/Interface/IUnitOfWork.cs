using System;
using System.Threading.Tasks;

namespace Vesuvio.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();

        bool SaveInTransaction();
    }
}