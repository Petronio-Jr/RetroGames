using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepositoryMock;
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IItemPedidoRepository> _itemPedidoRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorioMock;
    private readonly Mock<ISituacaoPedidoRepository> _situacaoPedidoRepositoryMock;

    public PedidoServiceTests()
    {
        _jogoRepositoryMock = new Mock<IJogoRepository>();
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _itemPedidoRepositoryMock = new Mock<IItemPedidoRepository>();
        _repositorioMock = new Mock<IRepositorySet>();

        _situacaoPedidoRepositoryMock = new Mock<ISituacaoPedidoRepository>();

        _repositorioMock
            .SetupGet(r => r.SituacaoPedido)
            .Returns(_situacaoPedidoRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.Jogo)
            .Returns(_jogoRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.Pedido)
            .Returns(_pedidoRepositoryMock.Object);

        _repositorioMock
            .SetupGet(r => r.ItemPedido)
            .Returns(_itemPedidoRepositoryMock.Object);
    }

    private PedidoService CriarService()
    {
        return new PedidoService(_repositorioMock.Object);
    }

    [Fact]
    public async Task Finalizar_EstoqueDisponivel_CriaPedidoEAtualizaEstoque()
    {
        // Arrange
        var jogo = new Jogo
        {
            Id = 1,
            Nome = "Super Mario World",
            Preco = 100m,
            Estoque = 10,
            Ativo = true
        };

        var itensSolicitados = new List<ItemPedidoSolicitadoServiceModel>
        {
            new ItemPedidoSolicitadoServiceModel
            {
                IdJogo = 1,
                Quantidade = 2
            }
        };

        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(jogo);

        _pedidoRepositoryMock
            .Setup(r => r.InserirAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(10);

        _jogoRepositoryMock
            .Setup(r => r.BaixarEstoqueAsync(1, 2))
            .ReturnsAsync(true);

        var service = CriarService();

        // Act
        var resultado = await service.FinalizarAsync(
            5,
            itensSolicitados
        );

        // Assert
        Assert.Equal(10, resultado.Id);
        Assert.Equal(5, resultado.IdUsuario);
        Assert.Equal(200m, resultado.Total);
        Assert.Equal(1, resultado.IdSituacaoPedido);

        _pedidoRepositoryMock.Verify(
            r => r.InserirAsync(
                It.Is<Pedido>(p =>
                    p.IdUsuario == 5 &&
                    p.Total == 200m &&
                    p.IdSituacaoPedido == 1
                )
            ),
            Times.Once
        );

        _itemPedidoRepositoryMock.Verify(
            r => r.InserirAsync(
                It.Is<List<ItemPedido>>(itens =>
                    itens.Count == 1 &&
                    itens[0].IdJogo == 1 &&
                    itens[0].Quantidade == 2 &&
                    itens[0].ValorUnitario == 100m
                )
            ),
            Times.Once
        );

        _jogoRepositoryMock.Verify(
            r => r.BaixarEstoqueAsync(1, 2),
            Times.Once
        );
    }

    [Fact]
    public async Task Finalizar_JogoInexistente_LancaErroENaoCriaPedido()
    {
        // Arrange
        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync((Jogo?)null);

        var itens = new List<ItemPedidoSolicitadoServiceModel>
        {
            new ItemPedidoSolicitadoServiceModel
            {
                IdJogo = 1,
                Quantidade = 1
            }
        };

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.FinalizarAsync(5, itens)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.JogoIndisponivel.Codigo,
            excecao.Erro.Codigo
        );

        _pedidoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Pedido>()),
            Times.Never
        );

        _itemPedidoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<List<ItemPedido>>()),
            Times.Never
        );

        _jogoRepositoryMock.Verify(
            r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Finalizar_EstoqueInsuficiente_LancaErroENaoCriaPedido()
    {
        // Arrange
        var jogo = new Jogo
        {
            Id = 1,
            Nome = "Super Mario World",
            Preco = 100m,
            Estoque = 2,
            Ativo = true
        };

        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(jogo);

        var itens = new List<ItemPedidoSolicitadoServiceModel>
        {
            new ItemPedidoSolicitadoServiceModel
            {
                IdJogo = 1,
                Quantidade = 5
            }
        };

        var service = CriarService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.FinalizarAsync(5, itens)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.EstoqueInsuficiente.Codigo,
            excecao.Erro.Codigo
        );

        _pedidoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Pedido>()),
            Times.Never
        );

        _itemPedidoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<List<ItemPedido>>()),
            Times.Never
        );

        _jogoRepositoryMock.Verify(
            r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never
        );
    }


    [Fact]
    public async Task ListarMeus_ComPedidosExistentes_RetornaPedidosComItensESituacao()
    {
        // Arrange
        var dataAntiga = new DateTime(2026, 11, 20);
        var dataNova = new DateTime(2026, 11, 24);

        var pedidos = new List<Pedido>
        {
            new Pedido
            {
                Id = 1,
                IdUsuario = 5,
                DataPedido = dataAntiga,
                Total = 100m,
                IdSituacaoPedido = 1
            },
            new Pedido
            {
                Id = 2,
                IdUsuario = 5,
                DataPedido = dataNova,
                Total = 200m,
                IdSituacaoPedido = 1
            }
        };

        var itens = new List<ItemPedido>
        {
            new ItemPedido
            {
                IdPedido = 1,
                IdJogo = 10,
                Quantidade = 1,
                ValorUnitario = 100m
            },
            new ItemPedido
            {
                IdPedido = 2,
                IdJogo = 20,
                Quantidade = 2,
                ValorUnitario = 100m
            }
        };

        var jogoAntigo = new Jogo
        {
            Id = 10,
            Nome = "Super Mario World"
        };

        var jogoNovo = new Jogo
        {
            Id = 20,
            Nome = "Sonic Mania"
        };

        _pedidoRepositoryMock
            .Setup(r => r.ListarPorUsuarioAsync(5))
            .ReturnsAsync(pedidos);

        _itemPedidoRepositoryMock
            .Setup(r => r.ListarPorPedidosAsync(
                It.Is<List<int>>(ids =>
                    ids.Count == 2 &&
                    ids.Contains(1) &&
                    ids.Contains(2)
                )
            ))
            .ReturnsAsync(itens);

        _situacaoPedidoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<SituacaoPedido>
            {
                new SituacaoPedido
                {
                    Id = 1,
                    Descricao = "Pendente"
                }
            });

        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(10))
            .ReturnsAsync(jogoAntigo);

        _jogoRepositoryMock
                .Setup(r => r.BuscarPorIdAsync(20))
                .ReturnsAsync(jogoNovo);

        var service = CriarService();

        // Act
        var resultado = await service.ListarMeusAsync(5);

        // Assert
        Assert.Equal(2, resultado.Count);

        // O pedido mais recente deve aparecer primeiro.
        Assert.Equal(2, resultado[0].Id);
        Assert.Equal(1, resultado[1].Id);

        Assert.Equal("Pendente", resultado[0].Situacao);
        Assert.Equal(200m, resultado[0].Total);

        Assert.Single(resultado[0].Itens);
        Assert.Equal("Sonic Mania", resultado[0].Itens[0].NomeJogo);
        Assert.Equal(2, resultado[0].Itens[0].Quantidade);
        Assert.Equal(100m, resultado[0].Itens[0].ValorUnitario);

        _pedidoRepositoryMock.Verify(
            r => r.ListarPorUsuarioAsync(5),
            Times.Once
        );
    }

}
