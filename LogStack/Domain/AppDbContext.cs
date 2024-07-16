using LogStack.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LogStack.Domain;

public sealed class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    public DbSet<Log> Logs { get; set; }

    public DbSet<TokenSecret> TokenSecrets { get; set; }

    public DbSet<User> Users { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<HasAccess> HasAccess { get; set; }


    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Ulid>()
            .HaveConversion<UlidToStringConverter>()
            .HaveConversion<UlidToBytesConverter>();
    }
}