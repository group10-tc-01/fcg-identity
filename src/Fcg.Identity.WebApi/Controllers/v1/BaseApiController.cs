using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Identity.WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected BaseApiController(IMediator mediator)
    {
        Mediator = mediator;
    }

    protected IMediator Mediator { get; }
}
