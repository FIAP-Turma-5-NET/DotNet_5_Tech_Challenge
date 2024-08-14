using FIAP_Contato.Application.Interface;
using FIAP_Contato.Application.Mapper;
using FIAP_Contato.Application.Service;
using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP_Contato.CrossCutting;

public static class Bootstrapper
{
    public static IServiceCollection AddRegisterServices(this IServiceCollection services)
    {
        services.AddSingleton(MapperConfiguration.RegisterMapping());
        services.AddScoped<IContatoDomainService, ContatoDomainService>();
        services.AddScoped<IContatoApplicationService, ContatoApplicationService>();
        services.AddScoped<IContatoRepository, ContatoRepository>();

        return services;
    }
}