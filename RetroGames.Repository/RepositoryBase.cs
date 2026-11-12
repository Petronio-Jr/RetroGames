using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);

        await conexao.OpenAsync();

        return conexao;
    }

    protected MySqlCommand CriarComando(
        MySqlConnection conexao,
        string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(
        MySqlCommand comando,
        string nome,
        object? valor)
    {
        comando.Parameters.AddWithValue(
            nome,
            valor ?? DBNull.Value
        );
    }

    protected async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando)
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