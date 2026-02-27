using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerce.Domain.Common
{
    public class PagedQuery
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string SortBy { get; init; } = "Nome";
        public string Direction { get; init; } = "asc";
    }
}
