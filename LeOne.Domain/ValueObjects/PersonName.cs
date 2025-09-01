namespace LeOne.Domain.ValueObjects
{
    public sealed record PersonName
    {
        public string FirstName { get; }
        public string LastName { get; }
        public PersonName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new DomainValidationException("FirstName is required");
            if (string.IsNullOrWhiteSpace(lastName)) throw new DomainValidationException("LastName is required");
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }
        public override string ToString() => $"{FirstName} {LastName}";
    }
}
