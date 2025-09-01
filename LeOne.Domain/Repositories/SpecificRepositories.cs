using LeOne.Domain.Entities;

namespace LeOne.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>;
    public interface IProductRepository : IRepository<Product>;
    public interface ISpaServiceRepository : IRepository<SpaService>;
    public interface ITransactionRepository : IRepository<Transaction>;
    public interface IReviewRepository : IRepository<Review>;
}
