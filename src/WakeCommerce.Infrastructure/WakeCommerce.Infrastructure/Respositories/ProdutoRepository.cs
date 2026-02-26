using Microsoft.EntityFrameworkCore;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Interfaces;
using WakeCommerce.Infrastructure.Context;

namespace WakeCommerce.Infrastructure.Respositories;

public class ProdutoRepository : IProdutoRepository
{
    private AppDbContext _produtoContext;
    public ProdutoRepository(AppDbContext context)
    {
        _produtoContext = context;
    }

    public async Task<Produto> CreateAsync(Produto product)
    {
        _produtoContext.Add(product);
        await _produtoContext.SaveChangesAsync();
        return product;
    }

    public async Task<IEnumerable<Produto>> GetByNameAsync(string name)
    {
        return await _produtoContext.Produtos
            .Where(p => EF.Functions.Like(p.Nome, $"%{name}%"))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Produto> GetByIdAsync(int? id)
    {
        return await _produtoContext.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Produto>> GetProdutosAsync()
    {
        return await _produtoContext.Produtos.ToListAsync();
    }

    public async Task<Produto> RemoveAsync(Produto product)
    {
        _produtoContext.Remove(product);
        await _produtoContext.SaveChangesAsync();
        return product;
    }

    public async Task<Produto> UpdateAsync(Produto product)
    {
        _produtoContext.Update(product);
        await _produtoContext.SaveChangesAsync();
        return product;
    }
}
