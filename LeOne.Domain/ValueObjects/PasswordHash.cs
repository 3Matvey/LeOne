namespace LeOne.Domain.ValueObjects
{
    public sealed record PasswordHash
    {
        public string Hash { get; }
        public string Salt { get; }
        private PasswordHash(string hash, string salt)
        {
            Hash = string.IsNullOrWhiteSpace(hash) ? throw new DomainValidationException("Hash is required") : hash;
            Salt = string.IsNullOrWhiteSpace(salt) ? throw new DomainValidationException("Salt is required") : salt;
        }
        public static PasswordHash Create(string hash, string salt) => new(hash, salt);
    }
}
