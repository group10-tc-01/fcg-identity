using Fcg.Identity.Application.UseCases.Items.CreateItem;
using Fcg.Identity.CommomTestsUtilities.Builders.Items;
using Fcg.Identity.CommomTestsUtilities.TestDoubles;
using FluentAssertions;
using Reqnroll;

namespace Fcg.Identity.FunctionalTests.Steps.Items;

[Binding]
public sealed class CreateItemStepDefinitions
{
    private CreateItemRequest _request = default!;
    private CreateItemResponse _response = default!;

    [Given("que eu possuo uma requisicao valida de item")]
    public void GivenQueEuPossuoUmaRequisicaoValidaDeItem()
    {
        _request = new CreateItemRequestBuilder().Build();
    }

    [When("eu executar o caso de uso de criacao")]
    public async Task WhenEuExecutarOCasoDeUsoDeCriacao()
    {
        var sut = new CreateItemUseCase(new InMemoryItemRepository(), new FakeUnitOfWork(), new FakeMessagePublisher());
        var result = await sut.Handle(_request, CancellationToken.None);
        _response = result.Value;
    }

    [Then("o item deve ser criado com sucesso")]
    public void ThenOItemDeveSerCriadoComSucesso()
    {
        _response.Id.Should().NotBeEmpty();
        _response.Name.Should().Be(_request.Name);
    }
}
