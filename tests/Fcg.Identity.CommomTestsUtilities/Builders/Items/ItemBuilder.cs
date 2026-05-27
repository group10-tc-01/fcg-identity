using Bogus;
using Fcg.Identity.Domain.Items;

namespace Fcg.Identity.CommomTestsUtilities.Builders.Items;

public sealed class ItemBuilder
{
    private readonly Faker _faker = new();

    public Item Build(string? name = null, decimal? price = null)
    {
        var result = Item.Create(
            name ?? _faker.Commerce.ProductName(),
            price ?? decimal.Parse(_faker.Commerce.Price(10, 500)));

        return result.Value;
    }
}
