using System.Data.Common;
using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    protected async Task<DbConnection> AbrirConexaoAsync()
    {
        DbConnection conexao =
            new MySqlConnection(_config.ConnectionString);

        await conexao.OpenAsync();

        return conexao;
    }

    protected DbCommand CriarComando(
        DbConnection conexao,
        string sql)
    {
        DbCommand comando = conexao.CreateCommand();
        comando.CommandText = sql;

        return comando;
    }

    protected void AdicionarParametro(
        DbCommand comando,
        string nome,
        object? valor)
    {
        DbParameter parametro = comando.CreateParameter();

        parametro.ParameterName = nome;
        parametro.Value = valor ?? DBNull.Value;

        comando.Parameters.Add(parametro);
    }

    protected async Task<int> ExecutarGravacaoAsync(
        DbCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException(
                "Já existe um registro com esses dados.",
                ex
            );
        }
    }
}
