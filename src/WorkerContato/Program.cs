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

        // Obtém configurações do launchSettings.json
        var fila = Environment.GetEnvironmentVariable("MassTransit_Filas_ContatoFila") ?? string.Empty;
        var servidor = Environment.GetEnvironmentVariable("MassTransit_Servidor") ?? string.Empty;
        var usuario = Environment.GetEnvironmentVariable("MassTransit_Usuario") ?? string.Empty;
        var senha = Environment.GetEnvironmentVariable("MassTransit_Senha") ?? string.Empty;
        var connectionString = Environment.GetEnvironmentVariable("Connection_String");

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
                cfg.ReceiveEndpoint(fila + "-Cadastrar", e =>
                {
                    e.ConfigureConsumer<ContatoConsumer>(context);
                });
                cfg.ReceiveEndpoint(fila + "-Atualizar", e =>
                {
                    e.ConfigureConsumer<ContatoConsumer>(context);
                });
                cfg.ReceiveEndpoint(fila + "-Deletar", e =>
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
