using Fcg.Identity.Domain.DonorProfiles;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Domain.DonorProfiles;

public sealed class DonorProfileTests
{
    [Fact]
    public void Create_Should_Return_Failure_When_KeycloakUserId_Is_Empty()
    {
        var result = DonorProfile.Create(string.Empty, "Maria Silva", "MARIA@EMAIL.COM", "12345678909");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DonorProfile.KeycloakUserIdRequired");
    }

    [Fact]
    public void Create_Should_Return_Failure_When_FullName_Is_Empty()
    {
        var result = DonorProfile.Create("keycloak-user-id", string.Empty, "MARIA@EMAIL.COM", "12345678909");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DonorProfile.FullNameRequired");
    }

    [Fact]
    public void Create_Should_Return_Failure_When_Email_Is_Empty()
    {
        var result = DonorProfile.Create("keycloak-user-id", "Maria Silva", string.Empty, "12345678909");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DonorProfile.EmailRequired");
    }

    [Fact]
    public void Create_Should_Return_Failure_When_Cpf_Is_Empty()
    {
        var result = DonorProfile.Create("keycloak-user-id", "Maria Silva", "MARIA@EMAIL.COM", string.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DonorProfile.CpfRequired");
    }

    [Fact]
    public void Create_Should_Normalize_Required_Profile_Data()
    {
        var result = DonorProfile.Create(" keycloak-user-id ", " Maria Silva ", " MARIA@EMAIL.COM ", " 12345678909 ");

        result.IsSuccess.Should().BeTrue();
        result.Value.KeycloakUserId.Should().Be("keycloak-user-id");
        result.Value.FullName.Should().Be("Maria Silva");
        result.Value.Email.Should().Be("maria@email.com");
        result.Value.Cpf.Should().Be("12345678909");
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Value.UpdatedAt.Should().BeNull();
    }
}
