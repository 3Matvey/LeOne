using LeOne.Domain.Entities;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class SpaServiceRepository(AppDbContext context) 
        : EfRepository<SpaService>(context);
}
