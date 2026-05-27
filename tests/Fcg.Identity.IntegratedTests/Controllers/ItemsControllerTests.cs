using System.Net;
using System.Net.Http.Json;
using Fcg.Identity.Application.UseCases.Items.CreateItem;
using Fcg.Identity.CommomTestsUtilities.Builders.Items;
using Fcg.Identity.IntegratedTests.Configurations;
using Fcg.Identity.WebApi.Models;
using FluentAssertions;

namespace Fcg.Identity.IntegratedTests.Controllers;

public sealed class ItemsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ItemsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Given_ValidRequest_When_PostIsCalled_Then_ShouldReturnCreated()
    {
        var request = new CreateItemRequestBuilder().Build();

        var response = await _client.PostAsJsonAsync("/api/v1/items", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<CreateItemResponse>>();
        payload.Should().NotBeNull();
        payload!.Data!.Id.Should().NotBeEmpty();
    }
}
