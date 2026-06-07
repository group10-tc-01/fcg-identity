using Fcs.Identity.Application.UseCases.Profiles.GetMe;
using Fcs.Identity.WebApi.Extensions;
using Fcs.Identity.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fcs.Identity.WebApi.Controllers.v1;

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

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(ApiResponse<GetMeResponse>.FromSuccess(result.Value));
    }
}
