using Fcg.Identity.Application.UseCases.Auth.Login;

namespace Fcg.Identity.WebApi.Authentication;

public static class AuthCookieWriter
{
    public static void AppendAuthCookies(HttpResponse response, LoginResponse loginResponse)
    {
        response.Cookies.Append(
            AuthCookieNames.AccessToken,
            loginResponse.AccessToken,
            CreateCookieOptions(TimeSpan.FromSeconds(loginResponse.ExpiresIn)));

        response.Cookies.Append(
            AuthCookieNames.RefreshToken,
            loginResponse.RefreshToken,
            CreateCookieOptions(TimeSpan.FromDays(7)));
    }

    public static void DeleteAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete(AuthCookieNames.AccessToken, CreateDeleteCookieOptions());
        response.Cookies.Delete(AuthCookieNames.RefreshToken, CreateDeleteCookieOptions());
    }

    private static CookieOptions CreateCookieOptions(TimeSpan maxAge) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        MaxAge = maxAge,
        Path = "/"
    };

    private static CookieOptions CreateDeleteCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Path = "/"
    };
}
