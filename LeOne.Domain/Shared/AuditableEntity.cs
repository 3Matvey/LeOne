namespace LeOne.Domain.Shared
{
    public abstract class AuditableEntity : Entity
    {
        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset? UpdatedAt { get; protected set; }
        public DateTimeOffset? DeletedAt { get; protected set; }
        public bool IsDeleted => DeletedAt.HasValue;

        protected void MarkCreated(DateTimeOffset now) => CreatedAt = now;
        public void MarkUpdated(DateTimeOffset now) => UpdatedAt = now;
        public void SoftDelete(DateTimeOffset now) => DeletedAt = now;
    }
}
