using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.UseCases.Auth.Login;

namespace Fcg.Identity.Application.UseCases.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<LoginResponse>;
