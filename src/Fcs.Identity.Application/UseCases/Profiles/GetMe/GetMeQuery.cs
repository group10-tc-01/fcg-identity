using Fcs.Identity.Application.Abstractions.Messaging;

namespace Fcs.Identity.Application.UseCases.Profiles.GetMe;

public sealed record GetMeQuery : IQuery<GetMeResponse>;
