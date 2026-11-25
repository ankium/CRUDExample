using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Fizzler;

namespace CRUDTest;

public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    #region
    [Fact]
    public async Task Index_ToReturnView()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

        // Assert
        string responseContent = await response.Content.ReadAsStringAsync();
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(responseContent);
        var document = htmlDocument.DocumentNode;
        document.QuerySelectorAll("table.persons").Should().NotBeNull();

    }
    #endregion
}
