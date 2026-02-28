using Microsoft.EntityFrameworkCore;
using WakeCommerce.Domain.Entities;
using WakeCommerce.Infrastructure.Context;

namespace WakeCommerce.Infrastructure.EntitiesConfiguration;

public static class DbSeeder
{
    private static readonly (string Nome, int Estoque, decimal Preco)[] ProdutosIniciais =
    {
        ("Notebook Dell Inspiron", 15, 3499.90m),
        ("Mouse Logitech MX Master", 50, 399.00m),
        ("Teclado Mecânico Keychron", 30, 599.00m),
        ("Monitor LG 27\" 4K", 20, 1899.00m),
        ("Webcam Logitech C920", 40, 699.00m)
    };

    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Produtos.AnyAsync())
            return;

        foreach (var (nome, estoque, preco) in ProdutosIniciais)
            context.Produtos.Add(new Produto(nome, estoque, preco));

        await context.SaveChangesAsync();
    }
}
