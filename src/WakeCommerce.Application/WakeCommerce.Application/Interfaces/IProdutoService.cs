using WakeCommerce.Application.DTO;

namespace WakeCommerce.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDTO>> GetProdutos();
        Task<ProdutoDTO> GetById(int? id);
        Task<IEnumerable<ProdutoDTO>> GetByName(string name);
        Task Add(ProdutoDTO produtoDto);
        Task Update(ProdutoDTO produtoDto);
        Task Remove(int? id);
    }
}
