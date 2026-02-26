using AutoMapper;
using WakeCommerce.Application.DTO;
using WakeCommerce.Domain.Entities;

namespace WakeCommerce.Application.Mappings
{
    public class DomainDTOMapping : Profile
    {
        public DomainDTOMapping()
        {
            CreateMap<Produto, ProdutoDTO>().ReverseMap();

        }
        
    }
}
