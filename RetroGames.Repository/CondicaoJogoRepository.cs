using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_condicao_jogo,
                ds_condicao_jogo
            FROM tb_condicao_jogo
            ORDER BY ds_condicao_jogo ASC;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        var condicoes = new List<CondicaoJogo>();

        while (await reader.ReadAsync())
        {
            condicoes.Add(new CondicaoJogo
            {
                Id = reader.GetInt32(reader.GetOrdinal("co_condicao_jogo")),
                Descricao = reader.GetString(reader.GetOrdinal("ds_condicao_jogo"))
            });
        }

        return condicoes;
    }
}