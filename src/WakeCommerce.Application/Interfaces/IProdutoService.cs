using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Domain.Common;

namespace WakeCommerce.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<PagedResult<ProdutoResponseDTO>> SearchAsync(ProdutoSearchDTO dto);
        Task<ProdutoResponseDTO> GetById(int id);
        Task Add(ProdutoCreateDTO dto);
        Task Update(int id, ProdutoUpdateDTO dto);
        Task Remove(int id);
    }
}
