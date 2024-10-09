using Bogus;

using FIAP_Contato.API.Controllers;
using FIAP_Contato.Application.Mapper;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Application.Service;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;

using Microsoft.AspNetCore.Mvc;

using Moq;

using System.ComponentModel.DataAnnotations;

using Xunit;

namespace FIAP_Contato.Test.Unit;

public class ContatoController
{
    private readonly Faker<ContatoModel> _faker;
    private readonly Faker<Contato> _fakerEntity;
    private AutoMapper.IMapper _mapper;

    public ContatoController()
    {
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

        _mapper = MapperConfiguration.RegisterMapping();
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void CadastrarContato_Sucesso()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.CadastrarScalarAsync(It.IsAny<Contato>())).ReturnsAsync(1);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        var model = _faker.Generate();

        // Act
        var validarAnnotation = Validator.TryValidateObject(model, new ValidationContext(model, null, null), null, true);
        var response = contatoController.CadastrarContato(model).Result;

        // Assert
        Assert.True(validarAnnotation);
        Assert.Equal("Cadastrado com sucesso!", ((ObjectResult)response).Value);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void CadastrarContato_Erro_Retorno_Banco()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.CadastrarAsync(It.IsAny<Contato>())).ReturnsAsync(1);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        var model = _faker.Generate();

        // Act        
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await contatoController.CadastrarContato(model));

        // Assert
        Assert.Equal("Erro ao Cadastrar!", ex.Result.Message);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void CadastrarContato_Erro_Contato_Existente()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        var listaDeEntidades = _fakerEntity.Generate(1);
        var listaDeModelos = _mapper.Map<List<ContatoModel>>(listaDeEntidades);

        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(listaDeEntidades);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act        
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await contatoController.CadastrarContato(listaDeModelos.FirstOrDefault()));

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
    public void AtualizarContato_Sucesso()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.AtualizarAsync(It.IsAny<Contato>())).ReturnsAsync(true);
        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(new List<Contato> { _fakerEntity });

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        var model = _faker.Generate();

        // Act
        var validarAnnotation = Validator.TryValidateObject(model, new ValidationContext(model, null, null), null, true);
        var response = contatoController.AtualizarContato(1, model).Result;

        // Assert
        Assert.True(validarAnnotation);
        Assert.Equal("Atualizado com sucesso!", ((ObjectResult)response).Value);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void AtualizarContato_Erro_Retorno_Banco()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.AtualizarAsync(It.IsAny<Contato>())).ReturnsAsync(false);
        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(new List<Contato> { _fakerEntity });

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        var model = _faker.Generate();

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await contatoController.AtualizarContato(1, model));
        Assert.Equal("Erro ao Atualizar!", ex.Result.Message);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void AtualizarContato_Erro_Contato_Nao_Existe()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.AtualizarAsync(It.IsAny<Contato>())).ReturnsAsync(false);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        var model = _faker.Generate();

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await contatoController.AtualizarContato(1, model));
        Assert.Equal("Contato não existe!", ex.Result.Message);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void DeletarContato_Sucesso()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.DeletarAsync(It.IsAny<Contato>())).ReturnsAsync(true);
        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(new List<Contato> { _fakerEntity });

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var response = contatoController.DeletarContato(1).Result;

        // Assert
        Assert.Equal("Deletado com sucesso!", ((ObjectResult)response).Value);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void DeletarContato_Erro_Retorno_Banco()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.DeletarAsync(It.IsAny<Contato>())).ReturnsAsync(false);
        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(new List<Contato> { _fakerEntity });

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await contatoController.DeletarContato(1));

        // Assert
        Assert.Equal("Erro ao Deletar!", ex.Result.Message);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void DeletarContato_Erro_Contato_Nao_Existe()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.DeletarAsync(It.IsAny<Contato>())).ReturnsAsync(false);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await contatoController.DeletarContato(1));

        // Assert
        Assert.Equal("Contato não existe!", ex.Result.Message);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void ObterTodosContatos_Sucesso()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        var listaDeEntidades = _fakerEntity.Generate(10);
        var listaDeModelos = _mapper.Map<List<ContatoModelResponse>>(listaDeEntidades);

        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(listaDeEntidades);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var response = (ObjectResult)contatoController.ObterTodosContatos(null).Result;
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
        var mockRepository = new Mock<IContatoRepository>();
        var listaDeEntidades = _fakerEntity.Generate(10);

        // Defina um DDD para filtrar
        var dddFiltrado = "11";
        var listaDeEntidadesFiltradas = listaDeEntidades.Where(e => e.DDD == dddFiltrado).ToList();
        var listaDeModelosFiltrados = _mapper.Map<List<ContatoModelResponse>>(listaDeEntidadesFiltradas);

        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(listaDeEntidades);

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var response = (ObjectResult)contatoController.ObterTodosContatos(dddFiltrado).Result;
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
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.ObterTodosAsync()).ReturnsAsync(new List<Contato>());

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var response = contatoController.ObterTodosContatos(null).Result;

        // Assert
        Assert.Equal(404, ((StatusCodeResult)response).StatusCode);
    }

    [Fact]
    [Trait("Categoria", "Unit")]
    public void GetAllContatos_Erro_Retorno_Banco()
    {
        // Arrange
        var mockRepository = new Mock<IContatoRepository>();
        mockRepository.Setup(c => c.ObterTodosAsync()).ThrowsAsync(new Exception("Erro na conexão!"));

        var contatoDomain = new ContatoDomainService(mockRepository.Object);
        var contatoService = new ContatoApplicationService(contatoDomain, _mapper);
        var contatoController = new API.Controllers.ContatoController(contatoService);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () => await contatoController.ObterTodosContatos(null));

        // Assert
        Assert.Equal("Erro na conexão!", ex.Result.Message);
    }
}
