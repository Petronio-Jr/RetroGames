using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_DevolveCondicoesEntreguesPeloRepository()
    {
        var condicoesEsperadas = new List<CondicaoJogo>
        {
            new CondicaoJogo { Id = 1, Descricao = "Novo" },
            new CondicaoJogo { Id = 2, Descricao = "Usado" },
            new CondicaoJogo { Id = 3, Descricao = "Lacrado" }
        };

        var condicaoJogoRepositoryMock =
            new Mock<ICondicaoJogoRepository>();

        condicaoJogoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(condicoesEsperadas);

        var repositorioMock = new Mock<IRepositorySet>();
        repositorioMock
            .SetupGet(r => r.CondicaoJogo)
            .Returns(condicaoJogoRepositoryMock.Object);

        var service = new CondicaoJogoService(
            repositorioMock.Object
        );

        var resultado = await service.ListarAsync();

        Assert.Equal(condicoesEsperadas, resultado);

        condicaoJogoRepositoryMock.Verify(
            r => r.ListarAsync(),
            Times.Once
        );
    }
}