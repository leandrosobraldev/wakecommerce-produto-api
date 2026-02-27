using Microsoft.EntityFrameworkCore;
using WakeCommerce.Domain.Entities;

namespace WakeCommerce.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.Property(p => p.Preco).HasPrecision(18, 2);
            entity.Property(p => p.Estoque).HasPrecision(18, 3);
        });
    }
}
