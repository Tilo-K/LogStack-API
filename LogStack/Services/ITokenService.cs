using LogStack.Domain.Models;
using LogStack.Entities;

namespace LogStack.Services;

public interface ITokenService
{
    Task<Token?> RenewToken(string refreshToken);
    Task<RefreshToken> CreateRefreshToken(Ulid userId);
}