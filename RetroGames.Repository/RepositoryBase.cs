using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    // Abre uma conexão com o banco de dados
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);

        await conexao.OpenAsync();

        return conexao;
    }

    // Cria um comando com o texto SQL informado
    protected MySqlCommand CriarComando(
        MySqlConnection conexao,
        string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    // Adiciona um parâmetro ao comando SQL
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

    // Executa comandos de gravação (INSERT ou UPDATE)
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