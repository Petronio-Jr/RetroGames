using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private const int SituacaoPendente = 1;

    private readonly IRepositorySet _repositorio;

    public PedidoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Pedido> FinalizarAsync(
        int idUsuario,
        List<ItemPedidoSolicitadoServiceModel> itensSolicitados)
    {
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            var jogos = new List<Jogo>();

            // 1. Busca os jogos e valida a disponibilidade.
            foreach (var item in itensSolicitados)
            {
                var jogo = await _repositorio.Jogo
                    .BuscarPorIdAsync(item.IdJogo);

                if (jogo is null || !jogo.Ativo)
                {
                    throw new BusinessRuleException(
                        BusinessErrors.JogoIndisponivel
                    );
                }

                if (jogo.Estoque < item.Quantidade)
                {
                    throw new BusinessRuleException(
                        BusinessErrors.EstoqueInsuficiente
                    );
                }

                jogos.Add(jogo);
            }

            // 2. Calcula o total usando os preços do banco.
            decimal total = itensSolicitados.Sum(item =>
            {
                var jogo = jogos.First(j => j.Id == item.IdJogo);

                return jogo.Preco * item.Quantidade;
            });

            // 3. Cria o pedido com situação Pendente.
            var pedido = new Pedido
            {
                DataPedido = DateTime.UtcNow,
                Total = total,
                IdUsuario = idUsuario,
                IdSituacaoPedido = SituacaoPendente
            };

            pedido.Id = await _repositorio.Pedido
                .InserirAsync(pedido);

            // 4. Cria os itens do pedido.
            var itensPedido = itensSolicitados
                .Select(item =>
                {
                    var jogo = jogos.First(
                        j => j.Id == item.IdJogo
                    );

                    return new ItemPedido
                    {
                        Quantidade = item.Quantidade,
                        ValorUnitario = jogo.Preco,
                        IdPedido = pedido.Id,
                        IdJogo = jogo.Id
                    };
                })
                .ToList();

            await _repositorio.ItemPedido
                .InserirAsync(itensPedido);

            // 5. Baixa o estoque de cada jogo.
            foreach (var item in itensSolicitados)
            {
                bool estoqueBaixado = await _repositorio.Jogo
                    .BaixarEstoqueAsync(
                        item.IdJogo,
                        item.Quantidade
                    );

                if (!estoqueBaixado)
                {
                    // Verifica se o jogo foi desativado ou removido
                    // enquanto a compra estava sendo processada.
                    var jogoAtual = await _repositorio.Jogo
                        .BuscarPorIdAsync(item.IdJogo);

                    if (jogoAtual is null || !jogoAtual.Ativo)
                    {
                        throw new BusinessRuleException(
                            BusinessErrors.JogoIndisponivel
                        );
                    }

                    throw new BusinessRuleException(
                        BusinessErrors.EstoqueInsuficiente
                    );
                }
            }

            // Confirma todas as operações.
            transactionScope.Complete();

            return pedido;
        }
    }

   
    public async Task<List<PedidoClienteServiceModel>> ListarMeusAsync(
    int idUsuario)
    {
        // Busca somente os pedidos pertencentes ao usuário.
        var pedidos = await _repositorio.Pedido
            .ListarPorUsuarioAsync(idUsuario);

        if (pedidos.Count == 0)
            return new List<PedidoClienteServiceModel>();

        // Busca os itens dos pedidos encontrados.
        var idsPedidos = pedidos.Select(p => p.Id).ToList();

        var itensPedido = await _repositorio.ItemPedido
            .ListarPorPedidosAsync(idsPedidos);

        // Busca as situações cadastradas.
        var situacoes = await _repositorio.SituacaoPedido
            .ListarAsync();

        // Busca os jogos associados aos itens.
        var idsJogos = itensPedido
            .Select(i => i.IdJogo)
            .Distinct()
            .ToList();

        var jogos = new List<Jogo>();

        foreach (var idJogo in idsJogos)
        {
            var jogo = await _repositorio.Jogo
                .BuscarPorIdAsync(idJogo);

            if (jogo != null)
                jogos.Add(jogo);
        }

        // Combina os dados utilizando LINQ.
        var resultado = pedidos
            .OrderByDescending(p => p.DataPedido)
            .Select(p => new PedidoClienteServiceModel
            {
                Id = p.Id,
                DataPedido = p.DataPedido,
                Total = p.Total,
                IdSituacaoPedido = p.IdSituacaoPedido,

                Situacao = situacoes
                    .FirstOrDefault(s => s.Id == p.IdSituacaoPedido)
                    ?.Descricao ?? string.Empty,

                Itens = itensPedido
                    .Where(i => i.IdPedido == p.Id)
                    .Select(i => new ItemPedidoClienteServiceModel
                    {
                        IdJogo = i.IdJogo,

                        NomeJogo = jogos
                            .FirstOrDefault(j => j.Id == i.IdJogo)
                            ?.Nome ?? string.Empty,

                        Quantidade = i.Quantidade,
                        ValorUnitario = i.ValorUnitario
                    })
                    .ToList()
            })
            .ToList();

        return resultado;
    }

}