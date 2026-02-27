using AutoMapper;
using WakeCommerce.Application.Common.Exceptions;
using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Application.Interfaces;
using WakeCommerce.Domain.Common;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Interfaces;

namespace WakeCommerce.Application.Services
{
    public sealed class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMapper _mapper;

        public ProdutoService(IMapper mapper, IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResult<ProdutoResponseDTO>> SearchAsync(ProdutoSearchDTO dto)
        {
            
            if (!string.IsNullOrWhiteSpace(dto.Nome))
            {
                var name = dto.Nome.Trim();

                if (name.Length < 3)
                    throw new BadRequestException("O parâmetro 'name' deve ter no mínimo 3 caracteres.");
            }

            var search = new ProdutoSearch
            {
                Name = dto.Nome?.Trim(),
                Page = dto.Page,
                PageSize = dto.PageSize,
                SortBy = dto.SortBy ?? "Nome",
                Direction = dto.Direction ?? "asc"
            };

            var result = await _produtoRepository.SearchAsync(search);

            if (result.Total == 0)
                throw new NotFoundException("Nenhum produto encontrado.");

            return new PagedResult<ProdutoResponseDTO>
            {
                Items = _mapper.Map<IReadOnlyList<ProdutoResponseDTO>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total
            };
        }

        public async Task<ProdutoResponseDTO> GetById(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Id inválido.");

            var entity = await _produtoRepository.GetByIdAsync(id);
            if (entity is null)
                throw new NotFoundException("Produto não encontrado.");

            return _mapper.Map<ProdutoResponseDTO>(entity);
        }

        public async Task Add(ProdutoCreateDTO dto)
        {
            var entity = _mapper.Map<Produto>(dto);
            await _produtoRepository.CreateAsync(entity);
        }

        public async Task Update(int id, ProdutoUpdateDTO dto)
        {
            if (id <= 0)
                throw new BadRequestException("Id inválido.");

            var existing = await _produtoRepository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException("Produto não encontrado.");

            
            existing.Update(dto.Nome, dto.Estoque, dto.Preco);

            await _produtoRepository.UpdateAsync(existing);
        }

        public async Task Remove(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Id inválido.");

            var existing = await _produtoRepository.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException("Produto não encontrado.");

            await _produtoRepository.RemoveAsync(existing);
        }
    }
}