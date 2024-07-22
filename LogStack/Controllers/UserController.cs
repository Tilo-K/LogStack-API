using LogStack.Domain.Models;
using LogStack.Entities;
using LogStack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogStack.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("getUsers")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = true)]
    public async Task<IEnumerable<User>> GetUsers()
    {
        return await userService.GetUsers();
    }

    [HttpPost("createUser")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = true)]
    public async Task<Ulid> CreateUser([FromQuery] string username, [FromQuery] string password,
        [FromQuery] string email, [FromQuery] bool admin = false)
    {
        return await userService.CreateUser(username, password, email, admin);
    }

    [HttpDelete("deleteUser")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = true)]
    public async Task DeleteUser([FromQuery] string userId)
    {
        await userService.DeleteUser(Ulid.Parse(userId));
    }
}