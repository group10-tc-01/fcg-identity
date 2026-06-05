using Fcs.Identity.Application.UseCases.Auth.Login;

namespace Fcs.Identity.WebApi.Authentication;

public static class AuthCookieWriter
{
    public static void AppendAuthCookies(HttpRequest request, HttpResponse response, LoginResponse loginResponse)
    {
        response.Cookies.Append(
            AuthCookieNames.AccessToken,
            loginResponse.AccessToken,
            CreateCookieOptions(request, TimeSpan.FromSeconds(loginResponse.ExpiresIn)));

        response.Cookies.Append(
            AuthCookieNames.RefreshToken,
            loginResponse.RefreshToken,
            CreateCookieOptions(request, TimeSpan.FromDays(7)));
    }

    public static void DeleteAuthCookies(HttpRequest request, HttpResponse response)
    {
        response.Cookies.Delete(AuthCookieNames.AccessToken, CreateDeleteCookieOptions(request));
        response.Cookies.Delete(AuthCookieNames.RefreshToken, CreateDeleteCookieOptions(request));
    }

    private static CookieOptions CreateCookieOptions(HttpRequest request, TimeSpan maxAge) => new()
    {
        HttpOnly = true,
        Secure = request.IsHttps,
        SameSite = request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
        MaxAge = maxAge,
        Path = "/"
    };

    private static CookieOptions CreateDeleteCookieOptions(HttpRequest request) => new()
    {
        HttpOnly = true,
        Secure = request.IsHttps,
        SameSite = request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
        Path = "/"
    };
}
