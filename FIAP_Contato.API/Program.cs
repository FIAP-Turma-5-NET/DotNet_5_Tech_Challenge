using FIAP_Contato.API.Middleware;
using FIAP_Contato.CrossCutting;
using FIAP_Contato.CrossCutting.Logger;

using MassTransit;

using Microsoft.OpenApi.Models;
using MySqlConnector;
using Prometheus;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://*:7109");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s => {
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP - Contato",
        Description = "Gestão de Contatos",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

//MassTransit - RabbitMQ
var servidor = configuration["MassTransit:Servidor"] ?? string.Empty;
var usuario = configuration["MassTransit:Usuario"] ?? string.Empty;
var senha = configuration["MassTransit:Senha"] ?? string.Empty;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(servidor, "/", h =>
        {
            h.Username(usuario);
            h.Password(senha);
        });

        cfg.ConfigureEndpoints(context);
    });
});

//Configuração para buscar a connection
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>((connection) => new MySqlConnection(connectionString));

// Configuration IoC
builder.Services.AddRegisterCommonServices();
builder.Services.AddRegisterServices();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(
    new CustomLoggerProvider(
        new CustomLoggerProviderConfiguration{ LogLevel = LogLevel.Information  }
        ));

var app = builder.Build();

app.UseMetricServer();

//Metricas Prometheus
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("host", context => context.Request.Host.Host);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseResponseHandleMiddleware();

app.Run();
