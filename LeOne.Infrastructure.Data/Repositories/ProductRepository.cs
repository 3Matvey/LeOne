using LeOne.Domain.Entities;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class ProductRepository(AppDbContext context)
        : EfRepository<Product>(context); 
}
