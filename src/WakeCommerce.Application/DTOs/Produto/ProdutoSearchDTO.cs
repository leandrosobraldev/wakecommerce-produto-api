using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerce.Application.DTOs.Produto
{
    public sealed class ProdutoSearchDTO
    {
        public string? Nome { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page deve ser maior que 0.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize deve estar entre 1 e 100.")]
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "Nome";

        public string? Direction { get; set; } = "asc";
    }
}
