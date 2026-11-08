using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        List<int>? idsJogos,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina)
    {
        await using var conexao = await AbrirConexaoAsync();

        var filtros = new List<string>
        {
            """
            (
                @busca IS NULL
                OR no_jogo COLLATE utf8mb4_unicode_ci
                   LIKE CONVERT(@busca USING utf8mb4) COLLATE utf8mb4_unicode_ci
            )
            """,
            "(@idPlataforma IS NULL OR co_plataforma = @idPlataforma)",
            "(@incluirInativos = 1 OR st_ativo = 1)"
        };

        await using var comandoTotal = CriarComando(conexao, "");

        AdicionarParametro(comandoTotal, "@busca",
            string.IsNullOrWhiteSpace(busca) ? null : $"%{busca}%");

        AdicionarParametro(comandoTotal, "@idPlataforma", idPlataforma);
        AdicionarParametro(comandoTotal, "@incluirInativos", incluirInativos ? 1 : 0);

        if (idsJogos != null)
        {
            if (idsJogos.Count == 0)
            {
                filtros.Add("1 = 0");
            }
            else
            {
                var parametrosIds = new List<string>();

                for (int i = 0; i < idsJogos.Count; i++)
                {
                    string nomeParametro = $"@idJogo{i}";
                    parametrosIds.Add(nomeParametro);

                    AdicionarParametro(
                        comandoTotal,
                        nomeParametro,
                        idsJogos[i]);
                }

                filtros.Add($"co_seq_jogo IN ({string.Join(", ", parametrosIds)})");
            }
        }

        string where = string.Join(" AND ", filtros);

        comandoTotal.CommandText = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            WHERE {where};
            """;

        int total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());

        int offset = (pagina - 1) * tamanhoPagina;

        await using var comando = CriarComando(conexao, $"""
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE {where}
            ORDER BY no_jogo ASC
            LIMIT @limite OFFSET @offset;
            """);

        AdicionarParametro(comando, "@busca",
            string.IsNullOrWhiteSpace(busca) ? null : $"%{busca}%");

        AdicionarParametro(comando, "@idPlataforma", idPlataforma);
        AdicionarParametro(comando, "@incluirInativos", incluirInativos ? 1 : 0);

        if (idsJogos != null)
        {
            for (int i = 0; i < idsJogos.Count; i++)
            {
                AdicionarParametro(comando, $"@idJogo{i}", idsJogos[i]);
            }
        }

        AdicionarParametro(comando, "@limite", tamanhoPagina);
        AdicionarParametro(comando, "@offset", offset);

        var jogos = new List<Jogo>();

        await using var reader = await comando.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            jogos.Add(MapearJogo(reader));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();

        await using var comando = CriarComando(conexao, """
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
            """);

        AdicionarParametro(comando, "@id", id);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapearJogo(reader);
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        await using var conexao = await AbrirConexaoAsync();

        await using var comando = CriarComando(conexao, """
            INSERT INTO tb_jogo (
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            ) VALUES (
                @nome,
                @descricao,
                @anoLancamento,
                @preco,
                @estoque,
                @ativo,
                UTC_TIMESTAMP(),
                @plataformaId,
                @condicaoJogoId
            );
            """);

        AdicionarParametrosJogo(comando, jogo);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        await using var conexao = await AbrirConexaoAsync();

        await using var comando = CriarComando(conexao, """
            UPDATE tb_jogo
            SET
                no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @plataformaId,
                co_condicao_jogo = @condicaoJogoId
            WHERE co_seq_jogo = @id;
            """);

        AdicionarParametrosJogo(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();

        await using var comando = CriarComando(conexao, """
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
            """);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private void AdicionarParametrosJogo(
        MySqlCommand comando,
        Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@plataformaId", jogo.PlataformaId);
        AdicionarParametro(comando, "@condicaoJogoId", jogo.CondicaoJogoId);
    }

    private static Jogo MapearJogo(MySqlDataReader reader)
    {
        return new Jogo
        {
            Id = reader.GetInt32(reader.GetOrdinal("co_seq_jogo")),
            Nome = reader.GetString(reader.GetOrdinal("no_jogo")),
            Descricao = reader.IsDBNull(reader.GetOrdinal("ds_jogo"))
                ? null
                : reader.GetString(reader.GetOrdinal("ds_jogo")),
            AnoLancamento = reader.GetInt32(reader.GetOrdinal("nu_ano_lancamento")),
            Preco = reader.GetDecimal(reader.GetOrdinal("vl_preco")),
            Estoque = reader.GetInt32(reader.GetOrdinal("qt_estoque")),
            Ativo = reader.GetBoolean(reader.GetOrdinal("st_ativo")),
            DataCadastro = reader.GetDateTime(reader.GetOrdinal("dt_cadastro")),
            PlataformaId = reader.GetInt32(reader.GetOrdinal("co_plataforma")),
            CondicaoJogoId = reader.GetInt32(reader.GetOrdinal("co_condicao_jogo"))
        };
    }
}