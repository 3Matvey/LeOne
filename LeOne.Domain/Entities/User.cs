using LeOne.Domain.ValueObjects;

namespace LeOne.Domain.Entities
{
    public class User : AuditableEntity
    {
        private User() { }

        public PersonName Name { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public PasswordHash Password { get; private set; } = null!;
        public UserRole Role { get; private set; } 

        public User(PersonName name, Email email, PasswordHash password, UserRole role)
            : base()
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public void ChangeName(PersonName name)
        { 
            Name = name; 
            MarkUpdated(DateTimeOffset.UtcNow); 
        }
        public void ChangeRole(UserRole role) 
        { 
            Role = role;
            MarkUpdated(DateTimeOffset.UtcNow);
        }
    }
}
