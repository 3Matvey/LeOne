using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;

namespace LeOne.Infrastructure.Data.Repositories
{
    public class ReviewRepository(AppDbContext context)
        : EfRepository<Review>(context), IReviewRepository;
}
