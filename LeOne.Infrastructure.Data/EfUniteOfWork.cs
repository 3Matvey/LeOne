using LeOne.Domain.Repositories;
using LeOne.Infrastructure.Data.Repositories;

namespace LeOne.Infrastructure.Data
{
    public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private readonly AppDbContext _context = context;

        private IUserRepository? _users;
        private IProductRepository? _products;
        private ISpaServiceRepository? _spaService;
        private IReviewRepository? _reviews;

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public ISpaServiceRepository SpaService => _spaService ??= new SpaServiceRepository(_context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

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
