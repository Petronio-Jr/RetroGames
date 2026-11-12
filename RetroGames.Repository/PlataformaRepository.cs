using RetroGames.Model.Model;
using System.Data.Common;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma ASC;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();

        while (await reader.ReadAsync())
        {
            plataformas.Add(MapearPlataforma(reader));
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapearPlataforma(reader);
    }

    public async Task<bool> ExisteComNomeAsync(
        string nome,
        int? idIgnorar = null)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM tb_plataforma
                WHERE no_plataforma COLLATE utf8mb4_unicode_ci =
                      CONVERT(@nome USING utf8mb4) COLLATE utf8mb4_unicode_ci
                  AND (
                      @idIgnorar IS NULL
                      OR co_seq_plataforma <> @idIgnorar
                  )
            ) AS existe;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = """
            INSERT INTO tb_plataforma (
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            )
            VALUES (
                @nome,
                @fabricante,
                @anoLancamento
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }


    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = """
            UPDATE tb_plataforma
            SET
                no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @anoLancamento
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
    }


    public async Task ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private static Plataforma MapearPlataforma(DbDataReader reader)
    {
        return new Plataforma
        {
            // co_seq_plataforma
            Id = reader.GetInt32(reader.GetOrdinal("co_seq_plataforma")),

            // no_plataforma
            Nome = reader.GetString(reader.GetOrdinal("no_plataforma")),

            // no_fabricante
            Fabricante = reader.GetString(reader.GetOrdinal("no_fabricante")),

            // nu_ano_lancamento
            AnoLancamento = reader.GetInt32(reader.GetOrdinal("nu_ano_lancamento"))
        };
    }
}
