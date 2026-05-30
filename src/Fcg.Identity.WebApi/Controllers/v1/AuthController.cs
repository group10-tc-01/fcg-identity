using Fcg.Identity.Application.UseCases.Donors.RegisterDonor;
using Fcg.Identity.WebApi.Extensions;
using Fcg.Identity.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Identity.WebApi.Controllers.v1;

[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("register/donor")]
    [ProducesResponseType(typeof(ApiResponse<RegisterDonorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterDonor(RegisterDonorRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterDonorCommand(
            request.FullName,
            request.Email,
            request.Cpf,
            request.Password);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            response => StatusCode(StatusCodes.Status201Created, ApiResponse<RegisterDonorResponse>.FromSuccess(response)),
            error => error.ToActionResult());
    }
}
