using Bogus;

using FIAP_Contato.API.Controllers;
using FIAP_Contato.Application.Mapper;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Application.Service;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;
using FIAP_Contato.Producer.Interface;
using FIAP_Contato.Producer.Producers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Moq;

using System.ComponentModel.DataAnnotations;

using Xunit;

namespace FIAP_Contato.Test.Unit;

public class ContatoControllerTests : IDisposable
{
    private readonly Faker<ContatoModel> _faker;
    private readonly Faker<Contato> _fakerEntity;
    private AutoMapper.IMapper _mapper;
    private Mock<IContatoRepository> _mockRepository;
    private Mock<IProducerService> _mockProducerService;
    private Mock<IConfiguration> _mockConfiguration;
    private ContatoApplicationService _contatoService;
    private ContatoController _contatoController;

    public ContatoControllerTests()
    {
        _mapper = MapperConfiguration.RegisterMapping();
        _mockRepository = new Mock<IContatoRepository>();
        _mockProducerService = new Mock<IProducerService>();
        _mockConfiguration = new Mock<IConfiguration>();

        var contatoDomain = new ContatoDomainService(_mockRepository.Object);
        var contadoProducer = new ContatoProducer(_mockProducerService.Object, _mockConfiguration.Object);
        _contatoService = new ContatoApplicationService(contatoDomain, _mapper, contadoProducer);
        _contatoController = new ContatoController(_contatoService);

        _faker = new Faker<ContatoModel>("pt_BR")
            .RuleFor(f => f.Nome, f => f.Name.FullName())
            .RuleFor(f => f.Telefone, f => f.Phone.PhoneNumberFormat())
            .RuleFor(f => f.Email, f => f.Internet.Email());

        _fakerEntity = new Faker<Contato>("pt_BR")
                .RuleFor(f => f.Id, f => 1)
                .RuleFor(f => f.Nome, f => f.Name.FullName())
                .RuleFor(f => f.Telefone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.DDD, f => f.PickRandom(new[] { "11", "21", "31", "12" }))
                .RuleFor(f => f.Email, f => f.Internet.Email());        
    }

    public void Dispose()
    {
       
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void CadastrarContato_Sucesso()
    {
        // Arrange
  
        _mockRepository.Setup(c => c.CadastrarScalarAsync(It.IsAny<Contato>())).ReturnsAsync(1);
        var model = _faker.Generate();

        // Act
        var validarAnnotation = Validator.TryValidateObject(model, new ValidationContext(model, null, null), null, true);
        var response = _contatoController.CadastrarContato(model).Result;

        // Assert
        Assert.True(validarAnnotation);
        Assert.Equal("Contato cadastrado com sucesso!", ((ObjectResult)response).Value);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void CadastrarContato_Erro_Contato_Existente()
    {
        // Arrange

        var listaDeEntidades = _fakerEntity.Generate(1);
        var listaDeModelos = _mapper.Map<List<ContatoModel>>(listaDeEntidades);
        _mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(listaDeEntidades);  

        // Act        
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
        await _contatoController.CadastrarContato(listaDeModelos.FirstOrDefault()));

        // Assert
        Assert.Equal("Contato já existe!", ex.Result.Message);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void CadastrarContato_Erro_Campos_Obrigatorios()
    {
        // Arrange
        var model = _faker.Generate();
        model.Nome = string.Empty;
        model.Telefone = string.Empty;
        model.Email = string.Empty;

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        // Act
        Validator.TryValidateObject(model, new ValidationContext(model, serviceProvider: null, items: null), validationResults, validateAllProperties: true);

        // Assert
        Assert.Equal("Nome é obrigatório!", validationResults[0].ErrorMessage);
        Assert.Equal("Telefone é obrigatório!", validationResults[1].ErrorMessage);
        Assert.Equal("E-mail é obrigatório!", validationResults[2].ErrorMessage);
    } 

    [Fact]
    [Trait("Categoria", "Unit")]
    public void ObterTodosContatos_Sucesso()
    {
        // Arrange
        var listaDeEntidades = _fakerEntity.Generate(10);
        var listaDeModelos = _mapper.Map<List<ContatoModelResponse>>(listaDeEntidades);

        _mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(listaDeEntidades); 

        // Act
        var response = (ObjectResult)_contatoController.ObterTodosContatos(null).Result;
        var responseModel = (List<ContatoModelResponse>)response.Value;

        // Assert
        bool todasEntidadesPresentes = listaDeModelos.All(m => responseModel.Any(r => r.Nome == m.Nome && r.Email == m.Email && r.Telefone == m.Telefone));
        Assert.True(todasEntidadesPresentes);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void ObterTodosContatos_Parametro_DDD_Sucesso()
    {
        // Arrange     
        var listaDeEntidades = _fakerEntity.Generate(10);

        // Defina um DDD para filtrar
        var dddFiltrado = "11";
        var listaDeEntidadesFiltradas = listaDeEntidades.Where(e => e.DDD == dddFiltrado).ToList();
        var listaDeModelosFiltrados = _mapper.Map<List<ContatoModelResponse>>(listaDeEntidadesFiltradas);

        _mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(listaDeEntidades);  

        // Act
        var response = (ObjectResult)_contatoController.ObterTodosContatos(dddFiltrado).Result;
        var responseModel = (List<ContatoModelResponse>)response.Value;

        // Assert
        bool todasEntidadesFiltradasPresentes = listaDeModelosFiltrados.All(m => responseModel.Any(r => r.Nome == m.Nome && r.Email == m.Email && r.Telefone == m.Telefone));

        Assert.True(todasEntidadesFiltradasPresentes);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(listaDeModelosFiltrados.Count, responseModel.Count);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void ObterTodosContatos_NenhumContatoEncontrado()
    {
        // Arrange      
        _mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(new List<Contato>());

        // Act
        var response = _contatoController.ObterTodosContatos(null).Result;

        // Assert
        Assert.Equal(404, ((StatusCodeResult)response).StatusCode);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void ObterTodosContatos_Erro_Retorno_Banco()
    {
        // Arrange       
        _mockRepository.Setup(c => c.ObterTodosAsync()).ThrowsAsync(new Exception("Erro na conexão!"));

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () => await _contatoController.ObterTodosContatos(null));

        // Assert
        Assert.Equal("Erro na conexão!", ex.Result.Message);
    }
}
