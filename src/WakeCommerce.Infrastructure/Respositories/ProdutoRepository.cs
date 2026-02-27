using Microsoft.EntityFrameworkCore;
using WakeCommerce.Domain.Common;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Domain.Interfaces;
using WakeCommerce.Infrastructure.Context;

namespace WakeCommerce.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Produto>> SearchAsync(ProdutoSearch search)
    {
        var page = search.Page <= 0 ? 1 : search.Page;
        var pageSize = search.PageSize <= 0 ? 10 : search.PageSize;

        if (pageSize > 100) pageSize = 100;

        var sortBy = (search.SortBy ?? "Nome").Trim().ToLowerInvariant();
        var direction = (search.Direction ?? "asc").Trim().ToLowerInvariant();
        var desc = direction == "desc";

        IQueryable<Produto> query = _context.Produtos
            .AsNoTracking();

        //Filtro por nome (prefix search = mais performático)
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            var name = search.Name.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.Nome, $"{name}%"));
        }

        //Ordenação segura (whitelist)
        query = sortBy switch
        {
            "preco" => desc
                ? query.OrderByDescending(p => p.Preco)
                : query.OrderBy(p => p.Preco),

            "estoque" => desc
                ? query.OrderByDescending(p => p.Estoque)
                : query.OrderBy(p => p.Estoque),

            _ => desc
                ? query.OrderByDescending(p => p.Nome)
                : query.OrderBy(p => p.Nome),
        };

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Produto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto> CreateAsync(Produto product)
    {
        _context.Produtos.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Produto product)
    {
        _context.Produtos.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Produto product)
    {
        _context.Produtos.Remove(product);
        await _context.SaveChangesAsync();
    }
}