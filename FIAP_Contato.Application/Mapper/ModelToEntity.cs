using AutoMapper;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Application.Mapper;

public class ModelToEntity : Profile
{
    public ModelToEntity()
    {
        CreateMap<ContatoModel,Contato>();
    }
}
