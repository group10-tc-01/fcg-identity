using Bogus;
using Fcg.Identity.Application.UseCases.Donors.RegisterDonor;
using Fcg.Identity.CommomTestsUtilities.Fakers.Shared;

namespace Fcg.Identity.CommomTestsUtilities.Builders.Donors;

public sealed class RegisterDonorCommandBuilder
{
    private readonly Faker _faker = new("pt_BR");
    private string _fullName;
    private string _email = EmailFaker.Generate();
    private string _cpf = CpfFaker.Generate();
    private string _password = "StrongPassword123!";

    public RegisterDonorCommandBuilder()
    {
        _fullName = _faker.Person.FullName;
    }

    public RegisterDonorCommandBuilder WithFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public RegisterDonorCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public RegisterDonorCommandBuilder WithCpf(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public RegisterDonorCommandBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public RegisterDonorCommand Build()
    {
        return new RegisterDonorCommand(_fullName, _email, _cpf, _password);
    }
}
