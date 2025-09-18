using LeOne.Domain.Entities;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class UserRepository(AppDbContext context)
        : EfRepository<User>(context); 
}
