using LeOne.Domain.Entities;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class ReviewRepository(AppDbContext context)
        : EfRepository<Review>(context);
}
