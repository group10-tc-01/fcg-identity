using Fcs.Identity.Application.UseCases.Auth.Login;
using Fcs.Identity.Application.UseCases.Auth.RefreshToken;
using Fcs.Identity.Application.UseCases.Donors.RegisterDonor;
using Fcs.Identity.WebApi.Authentication;
using Fcs.Identity.WebApi.Extensions;
using Fcs.Identity.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcs.Identity.WebApi.Controllers.v1;

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

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return StatusCode(StatusCodes.Status201Created, ApiResponse<RegisterDonorResponse>.FromSuccess(result.Value));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        AuthCookieWriter.AppendAuthCookies(Request, Response, result.Value);

        return Ok(ApiResponse<LoginResponse>.FromSuccess(result.Value));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = string.IsNullOrWhiteSpace(command.RefreshToken)
            ? Request.Cookies[AuthCookieNames.RefreshToken]
            : command.RefreshToken;

        var result = await _mediator.Send(command with { RefreshToken = refreshToken ?? string.Empty }, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        AuthCookieWriter.AppendAuthCookies(Request, Response, result.Value);

        return Ok(ApiResponse<LoginResponse>.FromSuccess(result.Value));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        AuthCookieWriter.DeleteAuthCookies(Request, Response);

        return NoContent();
    }
}
