namespace LeOne.Domain.Shared
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
       
        private protected Entity() 
        {
            Id = Guid.NewGuid();
        }

        // TODO add domain events  
    }
}