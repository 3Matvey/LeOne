namespace LeOne.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password, string salt);
    }
}
