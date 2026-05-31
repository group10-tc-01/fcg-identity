using Fcg.Identity.Application.UseCases.Profiles.GetMe;
using Fcg.Identity.WebApi.Extensions;
using Fcg.Identity.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Identity.WebApi.Controllers.v1;

[Authorize]
[Route("api/v{version:apiVersion}/me")]
public sealed class MeController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetMeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMeQuery(), cancellationToken);

        return result.Match<IActionResult>(
            response => Ok(ApiResponse<GetMeResponse>.FromSuccess(response)),
            error => error.ToActionResult());
    }
}
