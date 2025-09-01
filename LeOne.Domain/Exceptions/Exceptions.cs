namespace LeOne.Domain.Exceptions
{
    public class DomainException(string message) 
        : Exception(message);
    
    public class DomainValidationException(string message) 
        : DomainException(message);
}
