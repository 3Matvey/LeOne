using LeOne.Domain.ValueObjects;

namespace LeOne.Domain.Entities
{
    public class User : AuditableEntity
    {
        public PersonName Name { get; private set; }  
        public Email Email { get; private set; } 
        public PasswordHash Password { get; private set; } 
        public UserRole Role { get; private set; }

        public User(PersonName name, Email email, PasswordHash password, UserRole role, DateTimeOffset now)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            MarkCreated(now);
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
