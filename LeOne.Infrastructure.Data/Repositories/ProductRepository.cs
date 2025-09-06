using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class ProductRepository(AppDbContext context)
        : EfRepository<Product>(context); 
}
