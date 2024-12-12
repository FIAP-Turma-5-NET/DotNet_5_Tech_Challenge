using FIAP_Contato.Application.Interface;
using FIAP_Contato.Application.Mapper;
using FIAP_Contato.Application.Service;
using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;
using FIAP_Contato.Infrastructure.Producer.Service;
using FIAP_Contato.Producer.Interface;
using FIAP_Contato.Producer.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP_Contato.CrossCutting;

public static class Bootstrapper
{

    public static IServiceCollection AddRegisterCommonServices(this IServiceCollection services)
    {
        services.AddSingleton(MapperConfiguration.RegisterMapping());
        services.AddScoped<IContatoRepository, ContatoRepository>();

        return services;
    }
    public static IServiceCollection AddRegisterServices(this IServiceCollection services)
    {       
        services.AddScoped<IContatoDomainService, ContatoDomainService>();
        services.AddScoped<IContatoApplicationService, ContatoApplicationService>();        
        services.AddScoped<IProducerService, ProducerService>();
        services.AddScoped<IContatoProducer, ContatoProducer>();

        return services;
    }  
}