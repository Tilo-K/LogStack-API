namespace LogStack.Services;

public interface ITokenSecretService
{
    Task<string> GetSecret();
}