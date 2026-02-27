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
        Task<PagedResult<Produto>> SearchAsync(ProdutoSearch search);
        Task<Produto?> GetByIdAsync(int id);
        Task<Produto> CreateAsync(Produto product);
        Task UpdateAsync(Produto product);
        Task RemoveAsync(Produto product);
    }
}
