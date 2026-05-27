using Fcg.Identity.Application.UseCases.Items.CreateItem;
using Fcg.Identity.CommomTestsUtilities.Builders.Items;
using Fcg.Identity.CommomTestsUtilities.TestDoubles;
using Fcg.Identity.Domain.Results;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Application.UseCases.Items.CreateItem;

public sealed class CreateItemUseCaseTests
{
    [Fact]
    public async Task Given_ValidRequest_When_Handle_Then_ShouldCreateItem()
    {
        var repository = new InMemoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();
        var publisher = new FakeMessagePublisher();
        var request = new CreateItemRequestBuilder().Build();
        var sut = new CreateItemUseCase(repository, unitOfWork, publisher);

        var result = await sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Name.Should().Be(request.Name);
        publisher.PublishedMessages.Should().ContainSingle();
        unitOfWork.SaveChangesCalls.Should().Be(1);
    }

    [Fact]
    public async Task Given_DuplicatedName_When_Handle_Then_ShouldReturnConflictError()
    {
        var repository = new InMemoryItemRepository();
        var unitOfWork = new FakeUnitOfWork();
        var publisher = new FakeMessagePublisher();
        var request = new CreateItemRequestBuilder().Build();
        var sut = new CreateItemUseCase(repository, unitOfWork, publisher);

        var existingItem = new ItemBuilder().Build(name: request.Name);
        await repository.AddAsync(existingItem, CancellationToken.None);

        var result = await sut.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
