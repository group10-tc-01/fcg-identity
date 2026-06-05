using Fcs.Identity.Application.Abstractions.Messaging;

namespace Fcs.Identity.Application.UseCases.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
