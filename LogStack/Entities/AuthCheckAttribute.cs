using System.Security.Claims;
using LogStack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace LogStack.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AuthCheckAttribute : Attribute, IFilterFactory
{
    public bool NeedsToBeAdmin { get; set; }

    public AuthCheckAttribute(bool needsToBeAdmin = false)
    {
        NeedsToBeAdmin = needsToBeAdmin;
    }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var tokenSecretService = serviceProvider.GetRequiredService<ITokenSecretService>();
        return new AuthCheckFilter(tokenSecretService, this);
    }
}

public class AuthCheckFilter : IAsyncActionFilter
{
    private readonly ITokenSecretService _tokenSecretService;
    private readonly AuthCheckAttribute _attribute;

    public AuthCheckFilter(ITokenSecretService tokenSecretService, AuthCheckAttribute attribute)
    {
        _tokenSecretService = tokenSecretService;
        _attribute = attribute;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        bool checkPassed = await PerformCustomCheck(context);

        if (!checkPassed)
        {
            context.Result = new UnauthorizedObjectResult("Authentication failed!");
            return;
        }

        await next();
    }

    private async Task<bool> PerformCustomCheck(ActionExecutingContext context)
    {
        var attribute =
            context.ActionDescriptor.FilterDescriptors.FirstOrDefault(f => f.Filter is AuthCheckAttribute)?.Filter as
                AuthCheckAttribute;
        try
        {
            Token token = Token.FromHttpContext(context.HttpContext);

            if (attribute.NeedsToBeAdmin && !token.Admin)
            {
                return false;
            }

            string secret = await _tokenSecretService.GetSecret();
            return token.IsValid(secret);
        }
        catch
        {
            return false;
        }
    }
}