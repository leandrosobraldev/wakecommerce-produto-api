using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using WakeCommerce.Application.DTOs.Produto;
using Xunit;

namespace WakeCommerce.API.IntegrationTests;

public class ProdutosControllerIntegrationTests : IClassFixture<WakeCommerceWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public ProdutosControllerIntegrationTests(WakeCommerceWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarProdutos_SemFiltro_Retorna200ComListaPaginada()
    {
        var response = await _client.GetAsync("/api/v1/Produtos?page=1&pageSize=10");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PagedResult>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotNull(result!.Items);
        Assert.True(result.Total >= 0);
    }

    [Fact]
    public async Task GetById_ProdutoInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/v1/Produtos/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CriarProduto_Retorna201ComLocationERespostaSemId()
    {
        var dto = new ProdutoCreateDTO
        {
            Nome = "Produto Integração",
            Estoque = 5,
            Preco = 29.90m
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Produtos", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var created = await response.Content.ReadFromJsonAsync<ProdutoResponseDTO>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal("Produto Integração", created.Nome);
        Assert.Equal(29.90m, created.Preco);
        Assert.Equal(5, created.Estoque);
    }

    [Fact]
    public async Task Post_ProdutoComPrecoNegativo_Retorna400()
    {
        var dto = new { Nome = "Teste", Estoque = 1, Preco = -10m };
        var response = await _client.PostAsJsonAsync("/api/v1/Produtos", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_AtualizarProduto_Retorna204()
    {
        var createDto = new ProdutoCreateDTO
        {
            Nome = "Para Atualizar",
            Estoque = 2,
            Preco = 15.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Produtos", createDto);
        createResponse.EnsureSuccessStatusCode();
        var id = ObterIdDoLocation(createResponse.Headers.Location);

        var updateDto = new ProdutoUpdateDTO
        {
            Nome = "Atualizado",
            Estoque = 10,
            Preco = 19.90m
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/Produtos/{id}", updateDto);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/Produtos/{id}");
        getResponse.EnsureSuccessStatusCode();
        var updated = await getResponse.Content.ReadFromJsonAsync<ProdutoResponseDTO>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Atualizado", updated.Nome);
        Assert.Equal(19.90m, updated.Preco);
        Assert.Equal(10, updated.Estoque);
    }

    [Fact]
    public async Task Delete_RemoverProduto_Retorna204()
    {
        var createDto = new ProdutoCreateDTO
        {
            Nome = "Para Deletar",
            Estoque = 1,
            Preco = 1.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Produtos", createDto);
        createResponse.EnsureSuccessStatusCode();
        var id = ObterIdDoLocation(createResponse.Headers.Location);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Produtos/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/Produtos/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static int ObterIdDoLocation(Uri? location)
    {
        Assert.NotNull(location);
        var match = Regex.Match(location.ToString(), @"/Produtos/(\d+)$");
        Assert.True(match.Success, "Location deve conter o id do recurso.");
        return int.Parse(match.Groups[1].Value);
    }

    private class PagedResult
    {
        public IReadOnlyList<ProdutoResponseDTO> Items { get; set; } = null!;
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
