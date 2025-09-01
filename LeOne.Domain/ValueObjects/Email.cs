namespace LeOne.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }
        private Email(string value) => Value = value;
        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException("Email is required");
            try
            {
                var addr = new System.Net.Mail.MailAddress(value);
                if (addr.Address != value) throw new DomainValidationException($"Invalid email: {value}");
            }
            catch { throw new DomainValidationException($"Invalid email: {value}"); }
            return new Email(value.Trim());
        }
        public override string ToString() => Value;
    }
}
