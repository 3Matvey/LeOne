namespace LeOne.Domain.Entities
{
    public class Review : AuditableEntity
    {
        public Guid ByUserId { get; private set; }
        public Guid TransactionId { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public byte Mark { get; private set; } // звезды 1-5
        public string? Description { get; private set; }

        public Review(Guid byUserId, Guid transactionId, TransactionType type, byte mark, string? description, DateTimeOffset now)
        {
            if (mark is < 1 or > 5) 
                throw new DomainValidationException("Mark must be 1..5");

            Id = Guid.NewGuid();
            ByUserId = byUserId;
            TransactionId = transactionId;
            TransactionType = type;
            Mark = mark;
            Description = description?.Trim();

            MarkCreated(now);
        }
        public void Edit(byte mark, string? description)
        {
            if (mark is < 1 or > 5) 
                throw new DomainValidationException("Mark must be 1..5");

            Mark = mark;
            Description = description?.Trim();

            MarkUpdated(DateTimeOffset.UtcNow);
        }
    }
}
