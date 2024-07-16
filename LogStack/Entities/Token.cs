using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LogStack.Entities;

public class Token
{
    public Ulid UserId { get; set; }

    public string[] ProjectAccess { get; set; }

    public DateTime ExpirationDate { get; set; } = DateTime.Now.AddMinutes(30);

    public bool Admin { get; set; }

    public string Hash { get; set; }


    private string GetHash(string secret)
    {
        SHA512 sha512 = SHA512.Create();
        string content =
            $"{UserId.ToString()};{string.Join(",", ProjectAccess)};{Admin};{ExpirationDate.ToUniversalTime()};{secret}";

        byte[] hashBytes = sha512.ComputeHash(Encoding.ASCII.GetBytes(content));

        return Convert.ToBase64String(hashBytes);
    }

    public bool IsHashValid(string secret)
    {
        return GetHash(secret).Equals(Hash);
    }

    public static Token CreateToken(Ulid userId, string[] projectAccess, bool admin, string secret)
    {
        Token newToken = new Token();
        newToken.UserId = userId;
        newToken.ProjectAccess = projectAccess;
        newToken.Admin = admin;

        newToken.Hash = newToken.GetHash(secret);

        return newToken;
    }

    public static Token CreateFromBase64(string base64)
    {
        var options = new JsonSerializerOptions()
        {
            Converters =
            {
                new Cysharp.Serialization.Json.UlidJsonConverter()
            }
        };
    
        string json = Convert.FromBase64String(base64).ToString() ?? throw new Exception("Invalid Token");
        Token token = JsonSerializer.Deserialize<Token>(json, options) ?? throw new Exception("Invalid Token");

        return token;
    }

    public string ToBase64()
    {
        var options = new JsonSerializerOptions()
        {
            Converters =
            {
                new Cysharp.Serialization.Json.UlidJsonConverter()
            }
        };

        string json = JsonSerializer.Serialize(this, options);
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(json));
    }
}