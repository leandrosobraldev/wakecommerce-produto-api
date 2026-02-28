using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerce.Application.DTOs.Produto
{
    public sealed class ProdutoResponseDTO
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }
}
