using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
