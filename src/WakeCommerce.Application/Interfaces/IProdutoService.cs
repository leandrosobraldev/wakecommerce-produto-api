using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Domain.Common;

namespace WakeCommerce.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<PagedResult<ProdutoResponseDTO>> SearchAsync(ProdutoSearchDTO dto, CancellationToken cancellationToken = default);
        Task<ProdutoResponseDTO> GetById(int id, CancellationToken cancellationToken = default);
        Task<ProdutoCreateResult> Add(ProdutoCreateDTO dto, CancellationToken cancellationToken = default);
        Task Update(int id, ProdutoUpdateDTO dto, CancellationToken cancellationToken = default);
        Task Remove(int id, CancellationToken cancellationToken = default);
    }
}
