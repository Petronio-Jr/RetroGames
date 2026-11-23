using System.Data.Common;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task InserirAsync(List<ItemPedido> itens)
    {
        if (itens.Count == 0)
            return;

        await using var conexao = await AbrirConexaoAsync();

        var valores = new List<string>();

        await using var comando = CriarComando(conexao, "");

        for (int i = 0; i < itens.Count; i++)
        {
            string parametroQuantidade = $"@quantidade{i}";
            string parametroValorUnitario = $"@valorUnitario{i}";
            string parametroPedido = $"@idPedido{i}";
            string parametroJogo = $"@idJogo{i}";

            valores.Add(
                $"({parametroQuantidade}, {parametroValorUnitario}, " +
                $"{parametroPedido}, {parametroJogo})"
            );

            AdicionarParametro(
                comando,
                parametroQuantidade,
                itens[i].Quantidade
            );

            AdicionarParametro(
                comando,
                parametroValorUnitario,
                itens[i].ValorUnitario
            );

            AdicionarParametro(
                comando,
                parametroPedido,
                itens[i].IdPedido
            );

            AdicionarParametro(
                comando,
                parametroJogo,
                itens[i].IdJogo
            );
        }

        comando.CommandText = $"""
            INSERT INTO tb_item_pedido (
                qt_item,
                vl_unitario,
                co_pedido,
                co_jogo
            )
            VALUES {string.Join(", ", valores)};
            """;

        await ExecutarGravacaoAsync(comando);
    }

    public async Task<List<ItemPedido>> ListarPorPedidosAsync(
        List<int> idsPedidos)
    {
        var itens = new List<ItemPedido>();

        if (idsPedidos.Count == 0)
            return itens;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, "");

        var parametrosIds = new List<string>();

        for (int i = 0; i < idsPedidos.Count; i++)
        {
            string nomeParametro = $"@idPedido{i}";

            parametrosIds.Add(nomeParametro);

            AdicionarParametro(
                comando,
                nomeParametro,
                idsPedidos[i]
            );
        }

        comando.CommandText = $"""
            SELECT
                co_seq_item_pedido,
                qt_item,
                vl_unitario,
                co_pedido,
                co_jogo
            FROM tb_item_pedido
            WHERE co_pedido IN ({string.Join(", ", parametrosIds)})
            ORDER BY co_pedido, co_seq_item_pedido;
            """;

        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            itens.Add(MapearItemPedido(reader));
        }

        return itens;
    }

    private static ItemPedido MapearItemPedido(DbDataReader reader)
    {
        return new ItemPedido
        {
            // co_seq_item_pedido
            Id = reader.GetInt32(
                reader.GetOrdinal("co_seq_item_pedido")
            ),

            // qt_item
            Quantidade = Convert.ToInt32(
                reader["qt_item"]
            ),

            // vl_unitario
            ValorUnitario = reader.GetDecimal(
                reader.GetOrdinal("vl_unitario")
            ),

            // co_pedido
            IdPedido = reader.GetInt32(
                reader.GetOrdinal("co_pedido")
            ),

            // co_jogo
            IdJogo = reader.GetInt32(
                reader.GetOrdinal("co_jogo")
            )
        };
    }
}
