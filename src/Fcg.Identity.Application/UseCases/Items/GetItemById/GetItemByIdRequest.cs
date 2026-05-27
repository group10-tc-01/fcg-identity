using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.UseCases.Items.GetItemById;

public sealed record GetItemByIdRequest(Guid Id) : IQuery<GetItemByIdResponse>;
