using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepositoryMock;
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<ICondicaoJogoRepository> _condicaoJogoRepositoryMock;
    private readonly Mock<IGeneroRepository> _generoRepositoryMock;
    private readonly Mock<IJogoGeneroRepository> _jogoGeneroRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorioMock;
    private readonly Mock<IUsuarioAtualService> _usuarioAtualServiceMock;

    public JogoServiceTests()
    {
        _jogoRepositoryMock = new Mock<IJogoRepository>();
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _condicaoJogoRepositoryMock = new Mock<ICondicaoJogoRepository>();
        _generoRepositoryMock = new Mock<IGeneroRepository>();
        _jogoGeneroRepositoryMock = new Mock<IJogoGeneroRepository>();
        _repositorioMock = new Mock<IRepositorySet>();
        _usuarioAtualServiceMock = new Mock<IUsuarioAtualService>();

        _repositorioMock
            .SetupGet(r => r.Jogo)
            .Returns(_jogoRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.CondicaoJogo)
            .Returns(_condicaoJogoRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.Genero)
            .Returns(_generoRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.JogoGenero)
            .Returns(_jogoGeneroRepositoryMock.Object);

        _usuarioAtualServiceMock
            .SetupGet(u => u.EhAdministrador)
            .Returns(false);
    }

    private JogoService CriarService()
    {
        return new JogoService(
            _repositorioMock.Object,
            _usuarioAtualServiceMock.Object
        );
    }

    private Jogo CriarJogo(int id = 1)
    {
        return new Jogo
        {
            Id = id,
            Nome = "God of War",
            Descricao = "Jogo de ação e aventura",
            AnoLancamento = 2018,
            Preco = 199.90m,
            Estoque = 10,
            Ativo = true,
            IdPlataforma = 1,
            IdCondicaoJogo = 1
        };
    }

    private Plataforma CriarPlataforma()
    {
        return new Plataforma
        {
            Id = 1,
            Nome = "PlayStation 4",
            Fabricante = "Sony",
            AnoLancamento = 2013
        };
    }

    private CondicaoJogo CriarCondicao()
    {
        return new CondicaoJogo
        {
            Id = 1,
            Descricao = "Novo"
        };
    }

    private Genero CriarGenero(int id = 1)
    {
        return new Genero
        {
            Id = id,
            Nome = "Ação"
        };
    }

    // INSERIR

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaPlataformaNaoEncontradaENaoGrava()
    {
        // Arrange
        var jogo = CriarJogo();
        var generosIds = new List<int> { 1 };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync((Plataforma?)null);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(jogo, generosIds)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PlataformaNaoEncontrada.Codigo,
            excecao.Erro.Codigo
        );

        _jogoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaCondicaoNaoEncontradaENaoGrava()
    {
        // Arrange
        var jogo = CriarJogo();
        var generosIds = new List<int> { 1 };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(CriarPlataforma());

        _condicaoJogoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>());

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(jogo, generosIds)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.CondicaoNaoEncontrada.Codigo,
            excecao.Erro.Codigo
        );

        _jogoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaGeneroNaoEncontradoENaoGrava()
    {
        // Arrange
        var jogo = CriarJogo();
        var generosIds = new List<int> { 999 };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(CriarPlataforma());

        _condicaoJogoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                CriarCondicao()
            });

        _generoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero>
            {
                CriarGenero(1)
            });

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(jogo, generosIds)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.GeneroNaoEncontrado.Codigo,
            excecao.Erro.Codigo
        );

        _jogoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }

    // LISTAR

    [Fact]
    public async Task Listar_FiltrandoPorGenero_EnviaSomenteIdsDosJogosDoGenero()
    {
        // Arrange
        var vinculos = new List<JogoGenero>
        {
            new JogoGenero { IdJogo = 1, IdGenero = 2 },
            new JogoGenero { IdJogo = 2, IdGenero = 1 },
            new JogoGenero { IdJogo = 3, IdGenero = 2 },
            new JogoGenero { IdJogo = 1, IdGenero = 2 }
        };

        _jogoGeneroRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(vinculos);

        _jogoRepositoryMock
            .Setup(r => r.ListarAsync(
                null,
                null,
                It.IsAny<List<int>?>(),
                false,
                1,
                10))
            .ReturnsAsync((new List<Jogo>(), 0));

        var service = CriarService();

        // Act
        await service.ListarAsync(
            null,
            null,
            2,
            false,
            1,
            10
        );

        // Assert
        _jogoRepositoryMock.Verify(
            r => r.ListarAsync(
                null,
                null,
                It.Is<List<int>?>(
                    ids => ids != null &&
                           ids.Count == 2 &&
                           ids.Contains(1) &&
                           ids.Contains(3)
                ),
                false,
                1,
                10
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosENaoRestringeIds()
    {
        // Arrange
        _jogoRepositoryMock
            .Setup(r => r.ListarAsync(
                null,
                null,
                null,
                false,
                1,
                10))
            .ReturnsAsync((new List<Jogo>(), 0));

        var service = CriarService();

        // Act
        await service.ListarAsync(
            null,
            null,
            null,
            false,
            1,
            10
        );

        // Assert
        _jogoGeneroRepositoryMock.Verify(
            r => r.ListarAsync(),
            Times.Never
        );

        _jogoRepositoryMock.Verify(
            r => r.ListarAsync(
                null,
                null,
                null,
                false,
                1,
                10
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Listar_ComJogosNaPagina_RetornaNomeDaPlataformaEDescricaoDaCondicao()
    {
        // Arrange
        var jogo = CriarJogo();

        _jogoRepositoryMock
            .Setup(r => r.ListarAsync(
                null,
                null,
                null,
                false,
                1,
                10))
            .ReturnsAsync((new List<Jogo> { jogo }, 1));

        _plataformaRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>
            {
                CriarPlataforma()
            });

        _condicaoJogoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                CriarCondicao()
            });

        _generoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero>
            {
                CriarGenero()
            });

        _jogoGeneroRepositoryMock
            .Setup(r => r.ListarPorJogosAsync(
                It.Is<List<int>>(ids => ids.Contains(jogo.Id))))
            .ReturnsAsync(new List<JogoGenero>
            {
                new JogoGenero
                {
                    IdJogo = jogo.Id,
                    IdGenero = 1
                }
            });

        var service = CriarService();

        // Act
        var resultado = await service.ListarAsync(
            null,
            null,
            null,
            false,
            1,
            10
        );

        // Assert
        Assert.Single(resultado.Jogos);
        Assert.Equal(1, resultado.Total);

        Assert.Equal(
            "PlayStation 4",
            resultado.Jogos[0].NomePlataforma
        );

        Assert.Equal(
            "Novo",
            resultado.Jogos[0].DescricaoCondicao
        );
    }

    // BUSCAR POR ID

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaJogoNaoEncontrado()
    {
        // Arrange
        var id = 999;

        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync((Jogo?)null);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.BuscarPorIdAsync(id)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.JogoNaoEncontrado.Codigo,
            excecao.Erro.Codigo
        );
    }

    // DESATIVAR

    [Fact]
    public async Task Desativar_IdInexistente_LancaJogoNaoEncontradoENaoDesativa()
    {
        // Arrange
        var id = 999;

        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync((Jogo?)null);

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.DesativarAsync(id)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.JogoNaoEncontrado.Codigo,
            excecao.Erro.Codigo
        );

        _jogoRepositoryMock.Verify(
            r => r.DesativarAsync(id),
            Times.Never
        );
    }
}
