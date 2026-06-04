using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.Application.UseCases.Auth.RefreshToken;
using Fcg.Identity.Application.UseCases.Donors.RegisterDonor;
using Fcg.Identity.WebApi.Authentication;
using Fcg.Identity.WebApi.Extensions;
using Fcg.Identity.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Identity.WebApi.Controllers.v1;

[Consumes("application/json")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("register/donor")]
    [ProducesResponseType(typeof(ApiResponse<RegisterDonorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterDonor([FromBody] RegisterDonorCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            response => StatusCode(StatusCodes.Status201Created, ApiResponse<RegisterDonorResponse>.FromSuccess(response)),
            error => error.ToActionResult());
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            response =>
            {
                AuthCookieWriter.AppendAuthCookies(Request, Response, response);
                return Ok(ApiResponse<AuthSessionResponse>.FromSuccess(ToSessionResponse(response)));
            },
            error => error.ToActionResult());
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = string.IsNullOrWhiteSpace(command.RefreshToken)
            ? Request.Cookies[AuthCookieNames.RefreshToken]
            : command.RefreshToken;

        var result = await _mediator.Send(command with { RefreshToken = refreshToken ?? string.Empty }, cancellationToken);

        return result.Match<IActionResult>(
            response =>
            {
                AuthCookieWriter.AppendAuthCookies(Request, Response, response);
                return Ok(ApiResponse<AuthSessionResponse>.FromSuccess(ToSessionResponse(response)));
            },
            error => error.ToActionResult());
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        AuthCookieWriter.DeleteAuthCookies(Request, Response);

        return NoContent();
    }

    private static AuthSessionResponse ToSessionResponse(LoginResponse response) =>
        new(
            response.AccessToken,
            response.RefreshToken,
            response.ExpiresIn,
            response.TokenType);
}
