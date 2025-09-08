namespace LeOne.Domain.Entities
{
    public class Review : AuditableEntity
    {
        public Guid ByUserId { get; private set; }
        public Guid TransactionId { get; private set; }
        public TransactionType TransactionType { get; private set; }

        private byte _mark;
        public byte Mark
        {
            get => _mark;
            private set
            {
                if (value < 1 || value > 5)
                    throw new DomainValidationException("Mark must be 1..5");
                _mark = value;
            }
        }

        public string? Description { get; private set; }

        private Review() { }

        public Review(Guid byUserId, Guid transactionId, TransactionType transactionType, byte mark, string? description)
            : base()
        {
            ByUserId = byUserId;
            TransactionId = transactionId;
            TransactionType = transactionType;
            Mark = mark;
            Description = description;
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
