using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class SpaServiceRepository(AppDbContext context) 
        : EfRepository<SpaService>(context);
}
