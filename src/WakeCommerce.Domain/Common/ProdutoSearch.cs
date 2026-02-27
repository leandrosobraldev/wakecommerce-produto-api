using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerce.Domain.Common
{
    public sealed class ProdutoSearch : PagedQuery
    {
        public string? Name { get; init; }
    }
}
