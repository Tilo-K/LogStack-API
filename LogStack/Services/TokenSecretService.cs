using LogStack.Domain;
using LogStack.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LogStack.Services;

public class TokenSecretService(AppDbContext dbContext) : ITokenSecretService
{
    private static string? _cachedSecret;
    
    public async Task<string> GetSecret()
    {
        if (_cachedSecret != null) return _cachedSecret;

        List<TokenSecret> secrets = await dbContext.TokenSecrets.ToListAsync();
        if (secrets.Any())
        {
            _cachedSecret = secrets.First().Content;
            return secrets.First().Content;
        }

        TokenSecret secret = new TokenSecret()
        {
            Content = GenerateSecret()
        };

        await dbContext.TokenSecrets.AddAsync(secret);
        _cachedSecret = secret.Content;
        
        return secret.Content;
    }

    private string GenerateSecret()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!§$%&/()=?";
        return new string(Enumerable.Repeat(chars, 512)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}