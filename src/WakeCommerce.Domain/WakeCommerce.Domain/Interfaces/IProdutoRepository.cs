using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerce.Domain.Entities;

namespace WakeCommerce.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetProdutosAsync();
        Task<Produto> GetByIdAsync(int? id);
        Task<IEnumerable<Produto>> GetByNameAsync(string name);
        Task<Produto> CreateAsync(Produto product);
        Task<Produto> UpdateAsync(Produto product);
        Task<Produto> RemoveAsync(Produto product);
    }
}
