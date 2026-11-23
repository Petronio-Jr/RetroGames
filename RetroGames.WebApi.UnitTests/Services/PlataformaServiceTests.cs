using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<IJogoRepository> _jogoRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorioMock;

    public PlataformaServiceTests()
    {
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _jogoRepositoryMock = new Mock<IJogoRepository>();
        _repositorioMock = new Mock<IRepositorySet>();

        _repositorioMock
            .SetupGet(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.Jogo)
            .Returns(_jogoRepositoryMock.Object);
    }

    private PlataformaService CriarService()
    {
        return new PlataformaService(_repositorioMock.Object);
    }

    private Plataforma CriarPlataforma(int id = 1)
    {
        return new Plataforma
        {
            Id = id,
            Nome = "PlayStation 5",
            Fabricante = "Sony",
            AnoLancamento = 2020
        };
    }

    // LISTAR

    [Fact]
    public async Task Listar_SemPlataformas_RetornaListaVazia()
    {
        // Arrange
        _plataformaRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>());

        var service = CriarService();

        // Act
        var resultado = await service.ListarAsync();

        // Assert
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task Listar_ComPlataformas_RetornaLista()
    {
        // Arrange
        var plataformas = new List<Plataforma>
        {
            CriarPlataforma(1),
            new Plataforma
            {
                Id = 2,
                Nome = "Xbox Series X",
                Fabricante = "Microsoft",
                AnoLancamento = 2020
            }
        };

        _plataformaRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(plataformas);

        var service = CriarService();

        // Act
        var resultado = await service.ListarAsync();

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Same(plataformas, resultado);
    }

    // BUSCAR POR ID

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var id = 999;

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync((Plataforma?)null);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.BuscarPorIdAsync(id)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNaoEncontrada.Codigo,
            excecao.Erro.Codigo
        );
    }

    [Fact]
    public async Task BuscarPorId_IdExistente_RetornaPlataforma()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(plataforma);

        var service = CriarService();

        // Act
        var resultado = await service.BuscarPorIdAsync(plataforma.Id);

        // Assert
        Assert.Same(plataforma, resultado);
    }

    // INSERIR

    [Fact]
    public async Task Inserir_PlataformaValida_InserePlataforma()
    {
        // Arrange
        var plataforma = CriarPlataforma();
        plataforma.Nome = "  PlayStation 5  ";

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("PlayStation 5", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ReturnsAsync(10);

        var service = CriarService();

        // Act
        var resultado = await service.InserirAsync(plataforma);

        // Assert
        Assert.Equal(10, resultado.Id);
        Assert.Equal("PlayStation 5", resultado.Nome);

        _plataformaRepositoryMock.Verify(
            r => r.InserirAsync(plataforma),
            Times.Once
        );
    }

    [Fact]
    public async Task Inserir_NomeDuplicado_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync(plataforma.Nome, null))
            .ReturnsAsync(true);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Codigo,
            excecao.Erro.Codigo
        );

        _plataformaRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Plataforma>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Inserir_RegistroDuplicadoNoBanco_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync(plataforma.Nome, null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException());

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Codigo,
            excecao.Erro.Codigo
        );
    }

    // ALTERAR

    [Fact]
    public async Task Alterar_PlataformaValida_AlteraPlataforma()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(CriarPlataforma());

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync(
                plataforma.Nome,
                plataforma.Id))
            .ReturnsAsync(false);

        var service = CriarService();

        // Act
        var resultado = await service.AlterarAsync(plataforma);

        // Assert
        Assert.Same(plataforma, resultado);

        _plataformaRepositoryMock.Verify(
            r => r.AlterarAsync(plataforma),
            Times.Once
        );
    }

    [Fact]
    public async Task Alterar_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var plataforma = CriarPlataforma(999);

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync((Plataforma?)null);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.AlterarAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNaoEncontrada.Codigo,
            excecao.Erro.Codigo
        );

        _plataformaRepositoryMock.Verify(
            r => r.AlterarAsync(It.IsAny<Plataforma>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Alterar_NomeDuplicado_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(CriarPlataforma());

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync(
                plataforma.Nome,
                plataforma.Id))
            .ReturnsAsync(true);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.AlterarAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Codigo,
            excecao.Erro.Codigo
        );

        _plataformaRepositoryMock.Verify(
            r => r.AlterarAsync(It.IsAny<Plataforma>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Alterar_RegistroDuplicadoNoBanco_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(CriarPlataforma());

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync(
                plataforma.Nome,
                plataforma.Id))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.AlterarAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException());

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.AlterarAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNomeDuplicado.Codigo,
            excecao.Erro.Codigo
        );
    }

    // EXCLUIR

    [Fact]
    public async Task Excluir_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var id = 999;

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync((Plataforma?)null);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.ExcluirAsync(id)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNaoEncontrada.Codigo,
            excecao.Erro.Codigo
        );

        _plataformaRepositoryMock.Verify(
            r => r.ExcluirAsync(id),
            Times.Never
        );
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogos()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(plataforma);

        _jogoRepositoryMock
            .Setup(r => r.PossuiJogosAsync(plataforma.Id))
            .ReturnsAsync(true);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.ExcluirAsync(plataforma.Id)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaPossuiJogos.Codigo,
            excecao.Erro.Codigo
        );

        _plataformaRepositoryMock.Verify(
            r => r.ExcluirAsync(plataforma.Id),
            Times.Never
        );
    }

    [Fact]
    public async Task Excluir_PlataformaSemJogos_ExcluiPlataforma()
    {
        // Arrange
        var plataforma = CriarPlataforma();

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(plataforma.Id))
            .ReturnsAsync(plataforma);

        _jogoRepositoryMock
            .Setup(r => r.PossuiJogosAsync(plataforma.Id))
            .ReturnsAsync(false);

        var service = CriarService();

        // Act
        await service.ExcluirAsync(plataforma.Id);

        // Assert
        _plataformaRepositoryMock.Verify(
            r => r.ExcluirAsync(plataforma.Id),
            Times.Once
        );
    }
}
