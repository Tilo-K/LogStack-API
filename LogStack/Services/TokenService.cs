using LogStack.Domain;
using LogStack.Domain.Models;
using LogStack.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogStack.Services;

public class TokenService(AppDbContext dbContext, ITokenSecretService tokenSecretService) : ITokenService
{
    public async Task<Token?> RenewToken(string refreshToken)
    {
        RefreshToken? dbToken = await dbContext.RefreshTokens.Where(t => t.Token == refreshToken).FirstOrDefaultAsync();

        if (dbToken == null) return null;

        string secret = await tokenSecretService.GetSecret();

        User? user = await dbContext.Users.Where(u => u.Id == dbToken.UserId).FirstOrDefaultAsync();
        if (user == null) return null;

        if (dbToken.ExpirationDate <= DateTime.Now)
        {
            await dbContext.RefreshTokens.Where(t => t.Token == dbToken.Token).ExecuteDeleteAsync();
            return null;
        }

        HasAccess[] accesses = await dbContext.HasAccess.Where(a => a.UserId == user.Id).ToArrayAsync();
        string[] projects = accesses.Select(a => a.ProjectId.ToString()).ToArray();
        
        Token newToken = Token.CreateToken(dbToken.UserId, projects, user.Admin, secret);

        return newToken;
    }

    public async Task<RefreshToken> CreateRefreshToken(Ulid userId)
    {
        RefreshToken refreshToken = new RefreshToken();
        refreshToken.UserId = userId;
        refreshToken.Token = GenerateToken();
        refreshToken.ExpirationDate = DateTime.Now.AddMonths(1);

        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();
        
        return refreshToken;
    }

    private string GenerateToken()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return Ulid.NewUlid().ToString() + new string(Enumerable.Repeat(chars, 500)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}