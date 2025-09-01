namespace LeOne.Domain.Entities
{
    public class Product : AuditableEntity
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomainValidationException("Name is required");

                _name = value;
            }
        }
        private long _priceInCents;
        public long PriceInCents 
        { 
            get => _priceInCents;
            private set
            {
                if (value < 0)
                    throw new DomainValidationException("Price must be >= 0");

                _priceInCents = value;
            }
        }
        public string? Description { get; private set; }
        public DateTimeOffset? OrderedAt { get; private set; }

        public Product(string name, long priceInCents, string? description)
            : base()
        {
            Name = name;
            PriceInCents = priceInCents;
            Description = description;
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

