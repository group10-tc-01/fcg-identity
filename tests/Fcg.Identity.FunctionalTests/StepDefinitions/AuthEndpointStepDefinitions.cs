using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.Application.UseCases.Auth.RefreshToken;
using Fcg.Identity.Application.UseCases.Donors.RegisterDonor;
using Fcg.Identity.Application.UseCases.Profiles.GetMe;
using Fcg.Identity.CommomTestsUtilities.Builders.DonorProfiles;
using Fcg.Identity.CommomTestsUtilities.Builders.Donors;
using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Domain.Shared;
using Fcg.Identity.Domain.Shared.Results;
using Fcg.Identity.FunctionalTests.Configurations;
using Fcg.Identity.FunctionalTests.Support;
using Fcg.Identity.WebApi.Models;
using FluentAssertions;
using Reqnroll;

namespace Fcg.Identity.FunctionalTests.StepDefinitions;

[Binding]
public sealed class AuthEndpointStepDefinitions
{
    private readonly FunctionalWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private RegisterDonorCommand? _registerDonorCommand;
    private LoginCommand? _loginCommand;
    private RefreshTokenCommand? _refreshTokenCommand;
    private DonorProfile? _authenticatedDonorProfile;
    private HttpResponseMessage? _response;
    private string? _responseBody;

    public AuthEndpointStepDefinitions(FunctionalWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _factory.Reset();
        _response = null;
        _responseBody = null;
        _registerDonorCommand = null;
        _loginCommand = null;
        _refreshTokenCommand = null;
        _authenticatedDonorProfile = null;
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.KeycloakUserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.RoleHeader);
    }

    [Given("que tenho uma requisição válida para registrar um doador")]
    public void Given_QueTenhoUmaRequisicaoValidaParaRegistrarUmDoador()
    {
        _registerDonorCommand = new RegisterDonorCommandBuilder().Build();
    }

    [Given("que tenho uma requisição inválida para registrar um doador")]
    public void Given_QueTenhoUmaRequisicaoInvalidaParaRegistrarUmDoador()
    {
        _registerDonorCommand = new RegisterDonorCommandBuilder()
            .WithFullName(string.Empty)
            .WithEmail("invalid-email")
            .Build();
    }

    [Given("que tenho uma requisição válida de login")]
    public void Given_QueTenhoUmaRequisicaoValidaDeLogin()
    {
        _loginCommand = new LoginCommand("doador@email.com", "Password123!");
    }

    [Given("que o provedor de identidade recusará as credenciais")]
    public void Given_QueOProvedorDeIdentidadeRecusaraAsCredenciais()
    {
        _factory.IdentityProvider.ConfigureLoginResult(
            Error.Unauthorized("IdentityProvider.InvalidCredentials", "Invalid email or password."));
    }

    [Given("que tenho uma requisição de login com credenciais inválidas")]
    public void Given_QueTenhoUmaRequisicaoDeLoginComCredenciaisInvalidas()
    {
        _loginCommand = new LoginCommand("doador@email.com", "wrong-password");
    }

    [Given("que tenho uma requisição válida para renovar token")]
    public void Given_QueTenhoUmaRequisicaoValidaParaRenovarToken()
    {
        _refreshTokenCommand = new RefreshTokenCommand("refresh-token");
    }

    [Given("que o provedor de identidade recusará o refresh token")]
    public void Given_QueOProvedorDeIdentidadeRecusaraORefreshToken()
    {
        _factory.IdentityProvider.ConfigureRefreshTokenResult(
            Error.Unauthorized("IdentityProvider.InvalidRefreshToken", "Invalid refresh token."));
    }

    [Given("que tenho uma requisição com refresh token inválido")]
    public void Given_QueTenhoUmaRequisicaoComRefreshTokenInvalido()
    {
        _refreshTokenCommand = new RefreshTokenCommand("invalid-refresh-token");
    }

    [Given("que existe um perfil de doador autenticado")]
    public async Task Given_QueExisteUmPerfilDeDoadorAutenticado()
    {
        _authenticatedDonorProfile = new DonorProfileBuilder().Build();
        await _factory.DonorProfileRepository.AddAsync(_authenticatedDonorProfile);

        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.KeycloakUserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.RoleHeader);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.KeycloakUserIdHeader, _authenticatedDonorProfile.KeycloakUserId);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, IdentityRoles.Donor);
    }

    [When("eu enviar a requisição para registrar o doador")]
    public async Task When_EuEnviarARequisicaoParaRegistrarODoador()
    {
        _registerDonorCommand.Should().NotBeNull();

        await SendAsync(() => _client.PostAsJsonAsync("/api/v1/auth/register/donor", _registerDonorCommand));
    }

    [When("eu enviar a requisição de login")]
    public async Task When_EuEnviarARequisicaoDeLogin()
    {
        _loginCommand.Should().NotBeNull();

        await SendAsync(() => _client.PostAsJsonAsync("/api/v1/auth/login", _loginCommand));
    }

    [When("eu enviar a requisição para renovar token")]
    public async Task When_EuEnviarARequisicaoParaRenovarToken()
    {
        _refreshTokenCommand.Should().NotBeNull();

        await SendAsync(() => _client.PostAsJsonAsync("/api/v1/auth/refresh", _refreshTokenCommand));
    }

    [When("eu consultar meu perfil")]
    public async Task When_EuConsultarMeuPerfil()
    {
        await SendAsync(() => _client.GetAsync("/api/v1/me"));
    }

    [Then("a resposta deve ter status {int}")]
    public void Then_ARespostaDeveTerStatus(int statusCode)
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then("a resposta deve indicar sucesso")]
    public async Task Then_ARespostaDeveIndicarSucesso()
    {
        var payload = await ReadResponseAsync<object>();
        payload.Success.Should().BeTrue();
    }

    [Then("a resposta deve indicar falha")]
    public async Task Then_ARespostaDeveIndicarFalha()
    {
        var payload = await ReadResponseAsync<string>();
        payload.Success.Should().BeFalse();
    }

    [Then("a resposta deve conter os dados do doador registrado")]
    public async Task Then_ARespostaDeveConterOsDadosDoDoadorRegistrado()
    {
        _registerDonorCommand.Should().NotBeNull();

        var payload = await ReadResponseAsync<RegisterDonorResponse>();

        payload.Data.Should().NotBeNull();
        payload.Data!.Email.Should().Be(_registerDonorCommand!.Email);
    }

    [Then("a resposta deve conter o token de acesso")]
    public async Task Then_ARespostaDeveConterOTokenDeAcesso()
    {
        var payload = await ReadResponseAsync<LoginResponse>();

        payload.Data.Should().NotBeNull();
        payload.Data!.AccessToken.Should().Be("access-token");
    }

    [Then("a resposta deve conter o novo token de acesso")]
    public async Task Then_ARespostaDeveConterONovoTokenDeAcesso()
    {
        var payload = await ReadResponseAsync<LoginResponse>();

        payload.Data.Should().NotBeNull();
        payload.Data!.AccessToken.Should().Be("new-access-token");
    }

    [Then("a resposta deve conter o perfil do doador autenticado")]
    public async Task Then_ARespostaDeveConterOPerfilDoDoadorAutenticado()
    {
        _authenticatedDonorProfile.Should().NotBeNull();

        var payload = await ReadResponseAsync<GetMeResponse>();

        payload.Data.Should().NotBeNull();
        payload.Data!.Id.Should().Be(_authenticatedDonorProfile!.Id);
        payload.Data.Role.Should().Be(IdentityRoles.Donor);
    }

    [Then("a mensagem da resposta deve ser {string}")]
    public async Task Then_AMensagemDaRespostaDeveSer(string message)
    {
        var payload = await ReadResponseAsync<string>();

        payload.Message.Should().Be(message);
    }

    private async Task<ApiResponse<T>> ReadResponseAsync<T>()
    {
        await Task.CompletedTask;

        _responseBody.Should().NotBeNullOrWhiteSpace();

        var payload = JsonSerializer.Deserialize<ApiResponse<T>>(
            _responseBody!,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        payload.Should().NotBeNull();

        return payload!;
    }

    private async Task SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        _response = await send();
        _responseBody = await _response.Content.ReadAsStringAsync();
    }
}
