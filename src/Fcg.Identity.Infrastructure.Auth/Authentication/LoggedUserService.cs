using Fcg.Identity.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fcg.Identity.Infrastructure.Auth.Authentication;

public sealed class LoggedUserService : ILoggedUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggedUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetUserId()
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
