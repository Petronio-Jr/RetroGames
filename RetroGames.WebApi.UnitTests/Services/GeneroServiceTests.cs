using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task Listar_DevolveGenerosEntreguesPeloRepository()
    {
        var generosEsperados = new List<Genero>
        {
            new Genero { Id = 1, Nome = "Ação" },
            new Genero { Id = 2, Nome = "Aventura" },
            new Genero { Id = 3, Nome = "RPG" }
        };

        var generoRepositoryMock = new Mock<IGeneroRepository>();
        generoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(generosEsperados);

        var repositorioMock = new Mock<IRepositorySet>();
        repositorioMock
            .SetupGet(r => r.Genero)
            .Returns(generoRepositoryMock.Object);

        var service = new GeneroService(repositorioMock.Object);

        var resultado = await service.ListarAsync();

        Assert.Equal(generosEsperados, resultado);

        generoRepositoryMock.Verify(
            r => r.ListarAsync(),
            Times.Once
        );
    }
}