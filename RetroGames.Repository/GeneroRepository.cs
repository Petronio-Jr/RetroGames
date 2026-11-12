using MySqlConnector;
using RetroGames.Model.Model;
using System.Data.Common;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_seq_genero,
                no_genero
            FROM tb_genero
            ORDER BY no_genero ASC;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var reader = await comando.ExecuteReaderAsync();

        var generos = new List<Genero>();

        while (await reader.ReadAsync())
        {
            generos.Add(MapearGenero(reader));
        }

        return generos;
    }

    private static Genero MapearGenero(DbDataReader reader)
    {
        return new Genero
        {
            Id = reader.GetInt32(reader.GetOrdinal("co_seq_genero")),
            Nome = reader.GetString(reader.GetOrdinal("no_genero"))
        };
    }
}