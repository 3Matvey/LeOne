namespace LeOne.Domain.Entities
{
    public class SpaService : AuditableEntity
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

        private int _durationMinutes;
        public int DurationMinutes
        {
            get => _durationMinutes;
            private set
            {
                if (value <= 0)
                    throw new DomainValidationException("Duration must be > 0");

                _durationMinutes = value;
            }
        }

        public string? Description { get; private set; }

        public SpaService(string name, long priceInCents, int durationMinutes, string? description)
        {
            Name = name;
            PriceInCents = priceInCents;
            DurationMinutes = durationMinutes;
            Description = description;
        }

        public void ChangePrice(long newPriceInCents)
        {
            PriceInCents = newPriceInCents;
            MarkUpdated(DateTimeOffset.UtcNow);
        }

        public void ChangeDuration(int minutes)
        {
            DurationMinutes = minutes;
            MarkUpdated(DateTimeOffset.UtcNow);
        }
    }
}
