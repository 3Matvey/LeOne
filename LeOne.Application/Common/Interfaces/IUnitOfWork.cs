using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;

namespace LeOne.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Product> Products { get; }
        IRepository<SpaService> SpaService { get; }
        IRepository<Review> Reviews { get; }
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
    }
}
    