using System.Data.Common;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    public PedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<int> InserirAsync(Pedido pedido)
    {
        const string sql = """
            INSERT INTO tb_pedido (
                dt_pedido,
                vl_total,
                co_usuario,
                co_situacao_pedido
            )
            VALUES (
                @dataPedido,
                @total,
                @idUsuario,
                @idSituacaoPedido
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@dataPedido", pedido.DataPedido);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@idSituacaoPedido", pedido.IdSituacaoPedido);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        const string sql = """
            SELECT
                co_seq_pedido,
                dt_pedido,
                vl_total,
                co_usuario,
                co_situacao_pedido
            FROM tb_pedido
            WHERE co_usuario = @idUsuario
            ORDER BY dt_pedido DESC, co_seq_pedido DESC;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idUsuario", idUsuario);

        await using var reader = await comando.ExecuteReaderAsync();

        var pedidos = new List<Pedido>();

        while (await reader.ReadAsync())
        {
            pedidos.Add(MapearPedido(reader));
        }

        return pedidos;
    }

    private static Pedido MapearPedido(DbDataReader reader)
    {
        return new Pedido
        {
            // co_seq_pedido
            Id = reader.GetInt32(
                reader.GetOrdinal("co_seq_pedido")
            ),

            // dt_pedido
            DataPedido = reader.GetDateTime(
                reader.GetOrdinal("dt_pedido")
            ),

            // vl_total
            Total = reader.GetDecimal(
                reader.GetOrdinal("vl_total")
            ),

            // co_usuario
            IdUsuario = reader.GetInt32(
                reader.GetOrdinal("co_usuario")
            ),

            // co_situacao_pedido
            IdSituacaoPedido = Convert.ToInt32(
                reader["co_situacao_pedido"]
            )
        };
    }
}
