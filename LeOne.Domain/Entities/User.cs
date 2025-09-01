using LeOne.Domain.ValueObjects;

namespace LeOne.Domain.Entities
{
    public class User(PersonName name, Email email, PasswordHash password, UserRole role) 
        : AuditableEntity()
    {
        public PersonName Name { get; private set; } = name;
        public Email Email { get; private set; } = email;
        public PasswordHash Password { get; private set; } = password;
        public UserRole Role { get; private set; } = role;

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
