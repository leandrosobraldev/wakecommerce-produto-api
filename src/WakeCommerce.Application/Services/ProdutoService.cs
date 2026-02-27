using AutoMapper;
using WakeCommerce.Application.DTO;
using WakeCommerce.Application.Interfaces;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Interfaces;

namespace WakeCommerce.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private IProdutoRepository _produtoRepository;

        private readonly IMapper _mapper;
        public ProdutoService(IMapper mapper, IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository ??
                 throw new ArgumentNullException(nameof(produtoRepository));

            _mapper = mapper;
        }

        public async Task<IEnumerable<ProdutoDTO>> GetProdutos()
        {
            var productsEntity = await _produtoRepository.GetProdutosAsync();
            return _mapper.Map<IEnumerable<ProdutoDTO>>(productsEntity);
        }

        public async Task<IEnumerable<ProdutoDTO>> GetByName(string name)
        {
            var produtos = await _produtoRepository.GetByNameAsync(name);
            return _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        }

        public async Task<ProdutoDTO> GetById(int? id)
        {
            var productEntity = await _produtoRepository.GetByIdAsync(id);
            return _mapper.Map<ProdutoDTO>(productEntity);
        }

        public async Task Add(ProdutoDTO productDto)
        {
            var productEntity = _mapper.Map<Produto>(productDto);
            await _produtoRepository.CreateAsync(productEntity);
        }

        public async Task Update(ProdutoDTO productDto)
        {

            var productEntity = _mapper.Map<Produto>(productDto);
            await _produtoRepository.UpdateAsync(productEntity);
        }

        public async Task Remove(int? id)
        {
            var productEntity = _produtoRepository.GetByIdAsync(id).Result;
            await _produtoRepository.RemoveAsync(productEntity);
        }
    }
}
