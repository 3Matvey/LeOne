namespace LeOne.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public TransactionType Type { get; private set; }
        public Guid? ProductId { get; private set; }
        public Guid? SpaServiceId { get; private set; }
        public long TotalInCents { get; private set; }
        public DateTimeOffset OccurredAt { get; private set; }

        private Transaction(Guid userId, TransactionType type, long totalInCents, DateTimeOffset occurredAt)
        {
            if (totalInCents < 0) 
                throw new DomainValidationException("Total must be >= 0");

            Id = Guid.NewGuid();
            UserId = userId;
            Type = type;
            TotalInCents = totalInCents;
            OccurredAt = occurredAt;

            MarkCreated(occurredAt);
        }
        public static Transaction ForProduct(Guid userId, Guid productId, long totalInCents, DateTimeOffset at)
        {
            return new Transaction(userId, TransactionType.ProductPurchase, totalInCents, at) { ProductId = productId }; 
        }
        public static Transaction ForSpaService(Guid userId, Guid spaServiceId, long totalInCents, DateTimeOffset at)
        {
            return new Transaction(userId, TransactionType.SpaService, totalInCents, at) { SpaServiceId = spaServiceId }; 
        }
    }
}
