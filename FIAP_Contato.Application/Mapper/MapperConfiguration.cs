using AutoMapper;

namespace FIAP_Contato.Application.Mapper;

public class MapperConfiguration
{
    public static IMapper RegisterMapping()
    {
        return new AutoMapper.MapperConfiguration(mc =>
            {
                mc.AddProfile<EntityToModel>();
                mc.AddProfile<ModelToEntity>();
            }).CreateMapper();
    }
}