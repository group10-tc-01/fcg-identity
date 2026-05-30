using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Domain.Shared.Results;
using Fcg.Identity.Domain.Shared.ValueObjects;

namespace Fcg.Identity.Application.UseCases.Donors.RegisterDonor;

public sealed class RegisterDonorCommandHandler : ICommandHandler<RegisterDonorCommand, RegisterDonorResponse>
{
    private readonly IDonorProfileRepository _donorProfileRepository;
    private readonly IIdentityProvider _identityProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterDonorCommandHandler(
        IDonorProfileRepository donorProfileRepository,
        IIdentityProvider identityProvider,
        IUnitOfWork unitOfWork)
    {
        _donorProfileRepository = donorProfileRepository;
        _identityProvider = identityProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterDonorResponse>> Handle(RegisterDonorCommand command, CancellationToken cancellationToken)
    {
        var normalizedFullName = command.FullName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedFullName))
        {
            return Error.Validation("DonorProfile.FullNameRequired", "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Error.Validation("DonorProfile.PasswordRequired", "Password is required.");
        }

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            return emailResult.Error;
        }

        var cpfResult = Cpf.Create(command.Cpf);
        if (cpfResult.IsFailure)
        {
            return cpfResult.Error;
        }

        if (await _donorProfileRepository.ExistsByEmailAsync(emailResult.Value.Value, cancellationToken))
        {
            return Error.Conflict("DonorProfile.EmailAlreadyExists", "A donor profile with this email already exists.");
        }

        if (await _donorProfileRepository.ExistsByCpfAsync(cpfResult.Value.Value, cancellationToken))
        {
            return Error.Conflict("DonorProfile.CpfAlreadyExists", "A donor profile with this CPF already exists.");
        }

        var identityUserResult = await _identityProvider.CreateDonorAsync(
            new CreateDonorIdentityUserRequest(normalizedFullName, emailResult.Value.Value, command.Password),
            cancellationToken);

        if (identityUserResult.IsFailure)
        {
            return identityUserResult.Error;
        }

        var donorProfileResult = DonorProfile.Create(
            identityUserResult.Value.KeycloakUserId,
            normalizedFullName,
            emailResult.Value.Value,
            cpfResult.Value.Value);

        if (donorProfileResult.IsFailure)
        {
            return donorProfileResult.Error;
        }

        var donorProfile = donorProfileResult.Value;

        await _donorProfileRepository.AddAsync(donorProfile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterDonorResponse(
            donorProfile.Id,
            donorProfile.FullName,
            donorProfile.Email.Value,
            MaskCpf(donorProfile.Cpf.Value));
    }

    private static string MaskCpf(string cpf)
    {
        return $"***.***.***-{cpf[^2..]}";
    }
}
