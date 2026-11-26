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
        // 确认请求成功，HTTP 状态码为 2xx
        response.IsSuccessStatusCode.Should().BeTrue(); 

        // 读取并解析响应中的 HTML，以便检查页面结构。
        string responseContent = await response.Content.ReadAsStringAsync();
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(responseContent);
        var document = htmlDocument.DocumentNode;

        // 查询 class 为 persons 的 table 元素集合，并确认查询结果不为 null。
        document.QuerySelectorAll("table.persons").Should().NotBeNull();

    }
    #endregion
}
