using Fcg.Identity.Application.Abstractions.Authentication;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Domain.ManagerProfiles;
using Fcg.Identity.Domain.Shared;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Profiles.GetMe;

public sealed class GetMeQueryHandler : IQueryHandler<GetMeQuery, GetMeResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IDonorProfileRepository _donorProfileRepository;
    private readonly IManagerProfileRepository _managerProfileRepository;

    public GetMeQueryHandler(
        ICurrentUser currentUser,
        IDonorProfileRepository donorProfileRepository,
        IManagerProfileRepository managerProfileRepository)
    {
        _currentUser = currentUser;
        _donorProfileRepository = donorProfileRepository;
        _managerProfileRepository = managerProfileRepository;
    }

    public async Task<Result<GetMeResponse>> Handle(GetMeQuery query, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUser.KeycloakUserId))
        {
            return Error.Unauthorized("CurrentUser.Unauthenticated", "User is not authenticated.");
        }

        if (_currentUser.Roles.Contains(IdentityRoles.Donor))
        {
            var donorProfile = await _donorProfileRepository.GetByKeycloakUserIdAsync(_currentUser.KeycloakUserId, cancellationToken);
            if (donorProfile is null)
            {
                return Error.NotFound("Profile.NotFound", "Profile was not found.");
            }

            return new GetMeResponse(
                donorProfile.Id,
                donorProfile.KeycloakUserId,
                donorProfile.FullName,
                donorProfile.Email.Value,
                IdentityRoles.Donor);
        }

        if (_currentUser.Roles.Contains(IdentityRoles.Manager))
        {
            var managerProfile = await _managerProfileRepository.GetByKeycloakUserIdAsync(_currentUser.KeycloakUserId, cancellationToken);
            if (managerProfile is null)
            {
                return Error.NotFound("Profile.NotFound", "Profile was not found.");
            }

            return new GetMeResponse(
                managerProfile.Id,
                managerProfile.KeycloakUserId,
                managerProfile.FullName,
                managerProfile.Email.Value,
                IdentityRoles.Manager);
        }

        return Error.Unauthorized("CurrentUser.RoleNotAllowed", "User role is not allowed.");
    }
}
