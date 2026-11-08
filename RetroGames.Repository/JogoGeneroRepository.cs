using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarAsync()
    {
        await using var conexao = await AbrirConexaoAsync();

        await using var comando = CriarComando(conexao, """
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """);

        var vinculos = new List<JogoGenero>();

        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            vinculos.Add(MapearJogoGenero(reader));
        }

        return vinculos;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        List<int> idsJogos)
    {
        var vinculos = new List<JogoGenero>();

        if (idsJogos.Count == 0)
            return vinculos;

        await using var conexao = await AbrirConexaoAsync();

        var parametrosIds = new List<string>();

        await using var comando = CriarComando(conexao, "");

        for (int i = 0; i < idsJogos.Count; i++)
        {
            string nomeParametro = $"@idJogo{i}";
            parametrosIds.Add(nomeParametro);

            AdicionarParametro(comando, nomeParametro, idsJogos[i]);
        }

        comando.CommandText = $"""
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", parametrosIds)})
            ORDER BY co_jogo, co_genero;
            """;

        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            vinculos.Add(MapearJogoGenero(reader));
        }

        return vinculos;
    }

    public async Task InserirAsync(List<JogoGenero> vinculos)
    {
        if (vinculos.Count == 0)
            return;

        await using var conexao = await AbrirConexaoAsync();

        var valores = new List<string>();

        await using var comando = CriarComando(conexao, "");

        for (int i = 0; i < vinculos.Count; i++)
        {
            string parametroJogo = $"@jogo{i}";
            string parametroGenero = $"@genero{i}";

            valores.Add($"({parametroJogo}, {parametroGenero})");

            AdicionarParametro(comando, parametroJogo, vinculos[i].Id);
            AdicionarParametro(comando, parametroGenero, vinculos[i].GeneroId);
        }

        comando.CommandText = $"""
            INSERT INTO tb_jogo_genero (
                co_jogo,
                co_genero
            ) VALUES {string.Join(", ", valores)};
            """;

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int jogoId)
    {
        await using var conexao = await AbrirConexaoAsync();

        await using var comando = CriarComando(conexao, """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @jogoId;
            """);

        AdicionarParametro(comando, "@jogoId", jogoId);

        await comando.ExecuteNonQueryAsync();
    }

    private static JogoGenero MapearJogoGenero(
        MySqlConnector.MySqlDataReader reader)
    {
        return new JogoGenero
        {
            Id = reader.GetInt32(reader.GetOrdinal("co_jogo")),
            GeneroId = reader.GetInt32(reader.GetOrdinal("co_genero"))
        };
    }
}