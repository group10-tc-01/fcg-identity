using Bogus;
using Fcg.Identity.Application.UseCases.Items.CreateItem;

namespace Fcg.Identity.CommomTestsUtilities.Builders.Items;

public sealed class CreateItemRequestBuilder
{
    private readonly Faker<CreateItemRequest> _faker = new Faker<CreateItemRequest>()
        .CustomInstantiator(f => new CreateItemRequest(
            f.Commerce.ProductName(),
            decimal.Parse(f.Commerce.Price(10, 500))));

    public CreateItemRequest Build() => _faker.Generate();
}
