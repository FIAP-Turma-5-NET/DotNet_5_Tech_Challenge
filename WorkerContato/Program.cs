using FIAP_Contato.CrossCutting;
using FIAP_Contato.Consumer.Consumer;
using FIAP_Contato.Consumer.Interface;
using FIAP_Contato.Consumer.Service;
using FIAP_Contato.Worker;
using MassTransit;

using MySql.Data.MySqlClient;
using System.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // Obtém configurações do appsettings.json
        var fila = configuration["MassTransit:Filas:ContatoFila"] ?? string.Empty;    
        var servidor = configuration["MassTransit:Servidor"] ?? string.Empty;
        var usuario = configuration["MassTransit:Usuario"] ?? string.Empty;
        var senha = configuration["MassTransit:Senha"] ?? string.Empty;
        var connectionString = configuration.GetValue<string>("ConnectionString");

        // Registra serviços 
        services.AddScoped<IConsumerService, ConsumerService>();    
        services.AddRegisterCommonServices();

        //Configuração para buscar a connection        
        services.AddScoped<IDbConnection>((connection) => new MySqlConnection(connectionString));

        // Configura o Worker como serviço hospedado
        services.AddHostedService<Worker>();

        // Configura MassTransit
        services.AddMassTransit(x =>
        {
            // Registra o consumidor específico
            x.AddConsumer<ContatoConsumer>();

            // Configura o RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(servidor, "/", h =>
                {
                    h.Username(usuario);
                    h.Password(senha);
                });

                // Configura o endpoint para consumir mensagens da fila
                cfg.ReceiveEndpoint(fila, e =>
                {
                    e.ConfigureConsumer<ContatoConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

// Executa o host
host.Run();
