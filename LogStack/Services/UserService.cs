using System.Security.Cryptography;
using System.Text;
using LogStack.Domain;
using LogStack.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LogStack.Services;

public class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<User?> LoginUser(string username, string password)
    {
        string passwordHash = GetHashForPassword(password);

        User? user = await dbContext.Users.Where(u => u.Username == username && u.Password == passwordHash)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<Ulid> CreateUser(string username, string password, string email, bool admin = false)
    {
        User user = new()
        {
            Username = username,
            Password = GetHashForPassword(password),
            Email = email,
            Admin = admin
        };

        EntityEntry<User> result = await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return result.Entity.Id;
    }

    public async Task<bool> UserExists(string username)
    {
        User? resultUser = await dbContext.Users.Where(u => u.Username.Equals(username)).FirstOrDefaultAsync();

        return resultUser != null;
    }

    public async Task<User?> GetUserById(Ulid userId)
    {
        User? resultUser = await dbContext.Users.Where(u => u.Id.Equals(userId)).FirstOrDefaultAsync();

        return resultUser;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await dbContext.Users.ToListAsync();
    }

    public async Task DeleteUser(Ulid userId)
    {
        await dbContext.Users.Where(u => u.Id == userId).ExecuteDeleteAsync();
    }

    public async Task<bool> UserExists(User user)
    {
        return await UserExists(user.Username);
    }

    public async Task EnsureUser(string username, string password, string email, bool admin = false)
    {
        bool exists = await UserExists(username);
        if (exists) return;

        await CreateUser(username, password, email, admin);
    }
    
    private string GetHashForPassword(string value)
    {
        SHA512 sha512 = SHA512.Create();
        byte[] hashBytes = sha512.ComputeHash(Encoding.ASCII.GetBytes(value));
        return Convert.ToBase64String(hashBytes);
    }
}