using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LogStack.Domain.Models;
using LogStack.Entities;
using LogStack.Entities.Requests;
using LogStack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogStack.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ITokenService tokenService, IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginData loginData)
    {
        User? user = await userService.LoginUser(loginData.Username, loginData.Password);

        if (user == null) return Unauthorized();

        RefreshToken refreshToken = await tokenService.CreateRefreshToken(user.Id);
        Token? authToken = await tokenService.RenewToken(refreshToken.Token);

        if (authToken == null) throw new Exception("Error generating token");

        byte[] authBytes = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(authToken));
        LoginResult loginResult = new()
        {
            AuthToken = Convert.ToBase64String(authBytes),
            RefreshToken = refreshToken
        };

        return Ok(loginResult);
    }

    [HttpGet("me")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = false)]
    public async Task<ActionResult<User>> Me()
    {
        Token userToken = Token.FromHttpContext(HttpContext);
        User? user = await userService.GetUserById(userToken.UserId);

        if (user is not null)
        {
            user.Password = "";
        }
        
        return user is not null ? Ok(user) : NotFound();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<string>> RefreshToken([FromQuery] string refreshToken)
    {
        Token? newToken = await tokenService.RenewToken(refreshToken);
        if (newToken is null) return Unauthorized();

        return newToken.ToBase64();
    }
}