using Fcs.Identity.Application.Abstractions.Messaging;
using Fcs.Identity.Application.UseCases.Auth.Login;

namespace Fcs.Identity.Application.UseCases.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<LoginResponse>;
