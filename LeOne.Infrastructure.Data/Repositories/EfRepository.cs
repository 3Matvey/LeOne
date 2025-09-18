using LeOne.Domain.Repositories;
using LeOne.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class EfRepository<T>(AppDbContext context) 
        : IRepository<T>
        where T : Entity
    {
        protected readonly AppDbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(filter).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? filter,
            int? skip,
            int? take,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (filter is not null)
                query = query.Where(filter);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync(ct);
        }

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? filter = null,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (filter is not null)
                query = query.Where(filter);

            return await query.CountAsync(ct);
        }

        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}