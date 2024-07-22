using LogStack.Domain;
using LogStack.Entities;
using LogStack.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CORSCRINGE",
        b =>
        {
            b.WithOrigins("http://localhost")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
});

string? dbConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ??
                             builder.Configuration.GetConnectionString("DatabaseUrl");

if (dbConnectionString == null)
{
    throw new Exception("Missing Database connection string");
}

builder
    .Services
    .AddDbContext<AppDbContext>(config => config.UseNpgsql(dbConnectionString));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenSecretService, TokenSecretService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped <AuthCheckAttribute> ();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using (IServiceScope scope = app.Services.CreateScope())
{
    IUserService userService = scope.ServiceProvider.GetService<IUserService>() ?? throw new Exception("UserService not configured");
    IConfigurationSection config = app.Configuration.GetSection("DefaultUser");
    string? username = Environment.GetEnvironmentVariable("DEFAULT_USERNAME") ?? config.GetValue<string>("username");
    string? password = Environment.GetEnvironmentVariable("DEFAULT_USERNAME") ?? config.GetValue<string>("password");

    if (username != null && password != null)
    {
        await userService.EnsureUser(username, password, "default@email.com", true);
    }
}

app.UseCors("CORSCRINGE");
app.MapControllers();

app.Run();