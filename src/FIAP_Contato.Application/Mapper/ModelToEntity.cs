using AutoMapper;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Domain.Entity;

using Shared.Model;


namespace FIAP_Contato.Application.Mapper;

public class ModelToEntity : Profile
{
    public ModelToEntity()
    {
        CreateMap<ContatoModel,Contato>();

        CreateMap<ContatoMensagem,Contato>().ForMember(x => x.Telefone, cd => cd.MapFrom(map => map.Telefone));
        CreateMap<Contato, ContatoMensagem>().ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Telefone));


    }
}
