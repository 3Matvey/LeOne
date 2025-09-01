namespace LeOne.Domain.Shared
{
    public abstract class AuditableEntity : Entity
    {
        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset? UpdatedAt { get; protected set; }

        private protected AuditableEntity()
            : base()
        {
            MarkCreated(DateTimeOffset.UtcNow);
        }

        protected void MarkCreated(DateTimeOffset now) => CreatedAt = now;
        public void MarkUpdated(DateTimeOffset now) => UpdatedAt = now;
    }
}
