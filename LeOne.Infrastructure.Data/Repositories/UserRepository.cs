using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class UserRepository(AppDbContext context)
        : EfRepository<User>(context); 
}
