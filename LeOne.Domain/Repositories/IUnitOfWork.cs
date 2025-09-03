namespace LeOne.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        ISpaServiceRepository SpaService { get; }
        IReviewRepository Reviews { get; } 
        Task SaveChangesAsync(CancellationToken ct = default);
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default); 
    }
}
