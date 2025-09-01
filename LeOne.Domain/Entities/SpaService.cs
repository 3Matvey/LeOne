namespace LeOne.Domain.Entities
{
    public class SpaService : AuditableEntity
    {
        public string Name { get; private set; } = default!;
        public long PriceInCents { get; private set; }
        public int DurationMinutes { get; private set; }
        public string? Description { get; private set; }

        public SpaService(string name, long priceInCents, int durationMinutes, string? description, DateTimeOffset now)
        {
            if (priceInCents < 0) 
                throw new DomainValidationException("Price must be >= 0");
            if (durationMinutes <= 0) 
                throw new DomainValidationException("Duration must be > 0");

            Id = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new DomainValidationException("Name is required") : name.Trim();
            PriceInCents = priceInCents;
            DurationMinutes = durationMinutes;
            Description = description?.Trim();

            MarkCreated(now);
        }
        public void ChangePrice(long newPriceInCents)
        { 
            if (newPriceInCents < 0) 
                throw new DomainValidationException("Price must be >= 0"); 
            
            PriceInCents = newPriceInCents; 
            
            MarkUpdated(DateTimeOffset.UtcNow); 
        }
        public void ChangeDuration(int minutes)
        { 
            if (minutes <= 0) 
                throw new DomainValidationException("Duration must be > 0"); 
            
            DurationMinutes = minutes; 
            
            MarkUpdated(DateTimeOffset.UtcNow); 
        }
    }
}
