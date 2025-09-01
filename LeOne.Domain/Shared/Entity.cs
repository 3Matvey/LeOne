namespace LeOne.Domain.Shared
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
       
        private protected Entity() { } // for EF Core 

        // TODO add domain events  
    }
}