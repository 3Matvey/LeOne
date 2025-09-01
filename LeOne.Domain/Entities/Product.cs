namespace LeOne.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public string Name { get; private set; }
        public long PriceInCents { get; private set; }
        public string? Description { get; private set; }
        public DateTimeOffset? OrderedAt { get; private set; }

        public Product(string name, long priceInCents, string? description, DateTimeOffset now)
        {
            if (priceInCents < 0) throw new DomainValidationException("Price must be >= 0");
            Id = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new DomainValidationException("Name is required") : name.Trim();
            PriceInCents = priceInCents;
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

        public void MarkOrdered(DateTimeOffset at) => OrderedAt = at;
    }
}

