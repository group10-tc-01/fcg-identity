using Fcs.Identity.Application.Abstractions.Identity;
using Fcs.Identity.Application.Abstractions.Messaging;
using Fcs.Identity.CommomTestsUtilities.TestDoubles;
using Fcs.Identity.Domain.Abstractions;
using Fcs.Identity.Domain.DonorProfiles;
using Fcs.Identity.Domain.ManagerProfiles;
using Fcs.Identity.FunctionalTests.Support;
using Fcs.Identity.Infrastructure.SqlServer.Persistence;
using Fcs.Identity.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fcs.Identity.FunctionalTests.Configurations;

public sealed class FunctionalWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeIdentityProvider IdentityProvider { get; } = new();
    public InMemoryDonorProfileRepository DonorProfileRepository { get; } = new();
    public InMemoryManagerProfileRepository ManagerProfileRepository { get; } = new();
    public FakeUnitOfWork UnitOfWork { get; } = new();
    public FakeMessagePublisher MessagePublisher { get; } = new();

    public void Reset()
    {
        IdentityProvider.Reset();
        DonorProfileRepository.Reset();
        ManagerProfileRepository.Reset();
        UnitOfWork.Reset();
        MessagePublisher.Reset();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<FcsIdentityDbContext>>();
            services.RemoveAll<FcsIdentityDbContext>();
            services.RemoveAll<IDonorProfileRepository>();
            services.RemoveAll<IManagerProfileRepository>();
            services.RemoveAll<IUnitOfWork>();
            services.RemoveAll<IIdentityProvider>();
            services.RemoveAll<IMessagePublisher>();

            services.AddSingleton<IDonorProfileRepository>(DonorProfileRepository);
            services.AddSingleton<IManagerProfileRepository>(ManagerProfileRepository);
            services.AddSingleton<IUnitOfWork>(UnitOfWork);
            services.AddSingleton<IIdentityProvider>(IdentityProvider);
            services.AddSingleton<IMessagePublisher>(MessagePublisher);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.SchemeName, _ => { });
        });
    }
}
