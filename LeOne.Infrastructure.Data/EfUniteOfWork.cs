using LeOne.Application.Common.Interfaces;
using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;
using LeOne.Infrastructure.Data.Repositories;

namespace LeOne.Infrastructure.Data
{
    public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private readonly AppDbContext _context = context;

        private IRepository<User>? _users;
        private IRepository<Product>? _products;
        private IRepository<SpaService>? _spaService;
        private IRepository<Review>? _reviews;

        public IRepository<User> Users => _users ??= new UserRepository(_context);
        public IRepository<Product> Products => _products ??= new ProductRepository(_context);
        public IRepository<SpaService> SpaService => _spaService ??= new SpaServiceRepository(_context);
        public IRepository<Review> Reviews => _reviews ??= new ReviewRepository(_context);

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                await action(ct);
                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}
