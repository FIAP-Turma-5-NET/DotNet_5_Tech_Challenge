using AutoMapper;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Application.Mapper;

public class EntityToModel : Profile
{
    public EntityToModel()
    {
        CreateMap<Contato, ContatoModel>()
         .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src =>
             src.Telefone.Length == 9
                 ? $"({src.DDD}) {src.Telefone.Substring(0, 5)}-{src.Telefone.Substring(5)}"
                 : $"({src.DDD}) {src.Telefone.Substring(0, 4)}-{src.Telefone.Substring(4)}"
         ));
        CreateMap<Contato, ContatoModelResponse>()
         .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src =>
             src.Telefone.Length == 9
                 ? $"({src.DDD}) {src.Telefone.Substring(0, 5)}-{src.Telefone.Substring(5)}"
                 : $"({src.DDD}) {src.Telefone.Substring(0, 4)}-{src.Telefone.Substring(4)}"
         ));
    }
}
