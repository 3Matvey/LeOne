using System.Linq.Expressions;

namespace LeOne.Domain.Repositories
{
    public interface IRepository<T>
        where T : Entity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? filter, int? skip, int? take, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);

    }
}
