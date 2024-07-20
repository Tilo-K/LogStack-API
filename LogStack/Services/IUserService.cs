using LogStack.Domain.Models;

namespace LogStack.Services;

public interface IUserService
{
    Task<User?> LoginUser(string username, string password);
    Task<Ulid> CreateUser(string username, string password, string email, bool admin = false);
    Task<bool> UserExists(string username);
    Task<bool> UserExists(User user);
    Task EnsureUser(string username, string password, string email, bool admin = false);
    Task<User?> GetUserById(Ulid userId);
}