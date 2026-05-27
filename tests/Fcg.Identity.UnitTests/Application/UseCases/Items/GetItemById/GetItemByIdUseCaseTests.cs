using Fcg.Identity.Application.UseCases.Items.GetItemById;
using Fcg.Identity.CommomTestsUtilities.Builders.Items;
using Fcg.Identity.CommomTestsUtilities.TestDoubles;
using Fcg.Identity.Domain;
using FluentAssertions;
using Xunit;

namespace Fcg.Identity.UnitTests.Application.UseCases.Items.GetItemById;

public sealed class GetItemByIdUseCaseTests
{
    [Fact]
    public async Task Given_ExistingItem_When_Handle_Then_ShouldReturnItem()
    {
        var repository = new InMemoryItemRepository();
        var item = new ItemBuilder().Build();
        await repository.AddAsync(item, CancellationToken.None);
        var sut = new GetItemByIdUseCase(repository);

        var result = await sut.Handle(new GetItemByIdRequest(item.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(item.Id);
        result.Value.Name.Should().Be(item.Name);
    }

    [Fact]
    public async Task Given_UnknownItem_When_Handle_Then_ShouldReturnNotFoundError()
    {
        var sut = new GetItemByIdUseCase(new InMemoryItemRepository());

        var result = await sut.Handle(new GetItemByIdRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
