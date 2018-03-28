namespace Backend.Users.Domain.Services
{
    public interface IPasswordHasher
    {
        string ComputeHash(string password);
    }
}