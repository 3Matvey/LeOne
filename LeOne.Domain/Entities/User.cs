using LeOne.Domain.ValueObjects;

namespace LeOne.Domain.Entities
{
    public class User : AuditableEntity
    {
        private User() { }

        private string _firstName = string.Empty;
        public string FirstName
        { 
            get => _firstName;
            private set
            {
                if (string.IsNullOrWhiteSpace(value)) 
                    throw new DomainValidationException("FirstName is required");
                _firstName = value.Trim();
            }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomainValidationException("LastName is required");
                _lastName = value.Trim();
            }
        }

        public Email Email { get; private set; } = null!;
        public PasswordHash Password { get; private set; } = null!;
        public UserRole Role { get; private set; }

        public User(string firstName, string lastName, Email email, PasswordHash password, UserRole role)
            : base()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
        }

        public void ChangeName(string firstName, string lastName)
        { 
            FirstName = firstName;
            LastName = lastName;
            MarkUpdated(DateTimeOffset.UtcNow); 
        }
        public void ChangeRole(UserRole role) 
        { 
            Role = role;
            MarkUpdated(DateTimeOffset.UtcNow);
        }
    }
}
