using FIAP_Contato.API.Middleware;
using FIAP_Contato.CrossCutting;
using FIAP_Contato.CrossCutting.Log;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

//Configuração para buscar a connection
var connectionString = configuration.GetValue<string>("ConnectionString");
builder.Services.AddScoped<IDbConnection>((connection) => new MySqlConnection(connectionString));

// Configuration IoC
builder.Services.AddRegisterServices();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(
    new CustomLoggerProvider(
        new CustomLoggerProviderConfiguration{ LogLevel = LogLevel.Information  }
        ));

var app = builder.Build();

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
