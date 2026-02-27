using AutoMapper;
using WakeCommerce.Application.DTOs.Produto;
using WakeCommerce.Domain.Entities;

namespace WakeCommerce.Application.Mappings
{
    public class DomainDTOMapping : Profile
    {
        public DomainDTOMapping()
        {
            CreateMap<Produto, ProdutoCreateDTO>().ReverseMap();
            CreateMap<Produto, ProdutoResponseDTO>().ReverseMap();

        }
        
    }
}
