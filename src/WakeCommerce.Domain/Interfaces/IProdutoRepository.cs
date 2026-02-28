using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerce.Domain.Common;
using WakeCommerce.Domain.Entities;

namespace WakeCommerce.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<PagedResult<Produto>> SearchAsync(ProdutoSearch search, CancellationToken cancellationToken = default);
        Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Produto> CreateAsync(Produto product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Produto product, CancellationToken cancellationToken = default);
        Task RemoveAsync(Produto product, CancellationToken cancellationToken = default);
    }
}
