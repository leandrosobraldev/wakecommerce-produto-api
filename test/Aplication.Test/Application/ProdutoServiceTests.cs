using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WakeCommerce.Application.Common.Exceptions;
using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Application.Mappings;
using WakeCommerce.Application.Services;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Interfaces;
using Xunit;

namespace Aplication.Test.Application;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoServiceTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainDTOMapping>());
        _mapper = config.CreateMapper();
        _logger = Mock.Of<ILogger<ProdutoService>>();
    }

    [Fact]
    public async Task GetById_QuandoProdutoNaoExiste_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Produto?)null);
        var service = new ProdutoService(_mapper, _repositoryMock.Object, _logger);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetById(999));
    }

    [Fact]
    public async Task GetById_QuandoProdutoExiste_DeveRetornarDto()
    {
        var produto = new Produto("Notebook", 5, 3500m) { Id = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(produto);
        var service = new ProdutoService(_mapper, _repositoryMock.Object, _logger);

        var result = await service.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Notebook", result.Nome);
        Assert.Equal(3500m, result.Preco);
        Assert.Equal(5, result.Estoque);
    }

    [Fact]
    public async Task Add_ComDtoValido_DeveRetornarCreateProdutoResultComProdutoSemId()
    {
        var dto = new ProdutoCreateDTO { Nome = "Mouse", Estoque = 10, Preco = 99.90m };
        var entity = new Produto("Mouse", 10, 99.90m) { Id = 7 };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        var service = new ProdutoService(_mapper, _repositoryMock.Object, _logger);

        var result = await service.Add(dto);

        Assert.Equal(7, result.Id);
        Assert.NotNull(result.Produto);
        Assert.Equal("Mouse", result.Produto.Nome);
        Assert.Equal(99.90m, result.Produto.Preco);
        Assert.Equal(10, result.Produto.Estoque);
    }

    [Fact]
    public async Task Remove_QuandoProdutoNaoExiste_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Produto?)null);
        var service = new ProdutoService(_mapper, _repositoryMock.Object, _logger);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Remove(999));
    }
}
