using System.Data.Common;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class SituacaoPedidoRepository : RepositoryBase, ISituacaoPedidoRepository
{
    public SituacaoPedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_situacao_pedido,
                ds_situacao_pedido
            FROM tb_situacao_pedido
            ORDER BY co_situacao_pedido ASC;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        var situacoes = new List<SituacaoPedido>();

        while (await reader.ReadAsync())
        {
            situacoes.Add(MapearSituacaoPedido(reader));
        }

        return situacoes;
    }

    private static SituacaoPedido MapearSituacaoPedido(DbDataReader reader)
    {
        return new SituacaoPedido
        {
            // co_situacao_pedido
            Id = Convert.ToInt32(
                reader["co_situacao_pedido"]
            ),

            // ds_situacao_pedido
            Descricao = reader.GetString(
                reader.GetOrdinal("ds_situacao_pedido")
            )
        };
    }
}
