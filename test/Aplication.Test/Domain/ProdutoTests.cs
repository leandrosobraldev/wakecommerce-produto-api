using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Validation;
using Xunit;

namespace Aplication.Test.Domain;

public class ProdutoTests
{
    [Fact]
    public void Construtor_ComValoresValidos_DeveCriarProduto()
    {
        var produto = new Produto("Produto Teste", 10, 99.90m);
        Assert.Equal("Produto Teste", produto.Nome);
        Assert.Equal(10, produto.Estoque);
        Assert.Equal(99.90m, produto.Preco);
    }

    [Fact]
    public void Construtor_ComPrecoNegativo_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainExceptionValidation>(() =>
            new Produto("Produto", 1, -10m));
        Assert.Equal("Valor do preço inválido", ex.Message);
    }

    [Fact]
    public void Construtor_ComNomeVazio_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainExceptionValidation>(() =>
            new Produto("", 1, 10m));
        Assert.Equal("O nome é obrigatório", ex.Message);
    }

    [Fact]
    public void Construtor_ComEstoqueNegativo_DeveLancarDomainException()
    {
        var ex = Assert.Throws<DomainExceptionValidation>(() =>
            new Produto("Produto", -1, 10m));
        Assert.Equal("Estoque inválido", ex.Message);
    }

    [Fact]
    public void Update_ComPrecoNegativo_DeveLancarDomainException()
    {
        var produto = new Produto("Produto", 1, 10m);
        var ex = Assert.Throws<DomainExceptionValidation>(() =>
            produto.Update("Produto", 1, -5m));
        Assert.Equal("Valor do preço inválido", ex.Message);
    }
}
