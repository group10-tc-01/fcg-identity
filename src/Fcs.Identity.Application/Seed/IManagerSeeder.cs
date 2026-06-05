namespace Fcs.Identity.Application.Seed;

public interface IManagerSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
