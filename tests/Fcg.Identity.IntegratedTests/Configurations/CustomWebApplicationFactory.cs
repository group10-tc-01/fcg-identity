using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.CommomTestsUtilities.TestDoubles;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.Items;
using Fcg.Identity.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fcg.Identity.IntegratedTests.Configurations;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public InMemoryItemRepository Repository { get; } = new();
    public FakeUnitOfWork UnitOfWork { get; } = new();
    public FakeMessagePublisher Publisher { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IItemRepository>();
            services.RemoveAll<IUnitOfWork>();
            services.RemoveAll<IMessagePublisher>();

            services.AddSingleton<IItemRepository>(Repository);
            services.AddSingleton<IUnitOfWork>(UnitOfWork);
            services.AddSingleton<IMessagePublisher>(Publisher);
        });
    }
}
