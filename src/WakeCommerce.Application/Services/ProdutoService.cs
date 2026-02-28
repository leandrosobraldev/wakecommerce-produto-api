using AutoMapper;
using Microsoft.Extensions.Logging;
using WakeCommerce.Application.Common.Exceptions;
using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Application.Interfaces;
using WakeCommerce.Domain.Common;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Interfaces;

namespace WakeCommerce.Application.Services;

public sealed class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(IMapper mapper, IProdutoRepository produtoRepository, ILogger<ProdutoService> logger)
    {
        _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedResult<ProdutoResponseDTO>> SearchAsync(ProdutoSearchDTO dto, CancellationToken cancellationToken = default)
    {
        var search = new ProdutoSearch
        {
            Name = dto.Nome?.Trim(),
            Page = dto.Page,
            PageSize = dto.PageSize,
            SortBy = dto.SortBy ?? "Nome",
            Direction = dto.Direction ?? "asc"
        };

        var result = await _produtoRepository.SearchAsync(search, cancellationToken);

        _logger.LogDebug("SearchAsync: retornando {Count} produtos da página {Page}", result.Items.Count, result.Page);

        return new PagedResult<ProdutoResponseDTO>
        {
            Items = _mapper.Map<IReadOnlyList<ProdutoResponseDTO>>(result.Items),
            Page = result.Page,
            PageSize = result.PageSize,
            Total = result.Total
        };
    }

    public async Task<ProdutoResponseDTO> GetById(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            _logger.LogWarning("GetById: id inválido {Id}", id);
            throw new BadRequestException("Id inválido.");
        }

        var entity = await _produtoRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            _logger.LogWarning("GetById: produto não encontrado Id={Id}", id);
            throw new NotFoundException("Produto não encontrado.");
        }

        return _mapper.Map<ProdutoResponseDTO>(entity);
    }

    public async Task<ProdutoCreateResult> Add(ProdutoCreateDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Produto>(dto);
        var created = await _produtoRepository.CreateAsync(entity, cancellationToken);
        var response = _mapper.Map<ProdutoResponseDTO>(created);
        _logger.LogInformation("Produto criado via API: Id={Id}, Nome={Nome}", created.Id, created.Nome);
        return new ProdutoCreateResult(created.Id, response);
    }

    public async Task Update(int id, ProdutoUpdateDTO dto, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new BadRequestException("Id inválido.");

        var existing = await _produtoRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            _logger.LogWarning("Update: produto não encontrado Id={Id}", id);
            throw new NotFoundException("Produto não encontrado.");
        }

        existing.Update(dto.Nome, dto.Estoque, dto.Preco);
        await _produtoRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task Remove(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new BadRequestException("Id inválido.");

        var existing = await _produtoRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            _logger.LogWarning("Remove: produto não encontrado Id={Id}", id);
            throw new NotFoundException("Produto não encontrado.");
        }

        await _produtoRepository.RemoveAsync(existing, cancellationToken);
    }
}