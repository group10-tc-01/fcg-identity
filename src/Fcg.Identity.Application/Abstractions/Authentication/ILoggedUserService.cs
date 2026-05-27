namespace Fcg.Identity.Application.Abstractions.Authentication;

public interface ILoggedUserService
{
    Guid? GetUserId();
}
