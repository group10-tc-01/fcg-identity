using Fcg.Identity.Application.UseCases.Items.CreateItem;
using Fcg.Identity.Application.UseCases.Items.GetItemById;
using Fcg.Identity.WebApi.Extensions;
using Fcg.Identity.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Identity.WebApi.Controllers.v1;

public sealed class ItemsController : BaseApiController
{
    public ItemsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id, version = "1" },
            ApiResponse<CreateItemResponse>.FromSuccess(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetItemByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetItemByIdRequest(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(ApiResponse<GetItemByIdResponse>.FromSuccess(result.Value));
    }
}
