using System.Data.Common;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    public UsuarioRepository(RepositoryConfig config)
        : base(config)
    {
    }

public async Task<Usuario?> BuscarPorEmailAsync(string email)
{
    const string sql = """
        SELECT
            co_seq_usuario,
            no_usuario,
            ds_email,
            ds_senha_hash,
            st_ativo,
            dt_cadastro,
            co_perfil
        FROM tb_usuario
        WHERE ds_email =
            CONVERT(@email USING utf8mb4) COLLATE utf8mb4_unicode_ci;
        """;

    await using var conexao = await AbrirConexaoAsync();
    await using var comando = CriarComando(conexao, sql);

    AdicionarParametro(comando, "@email", email);

    await using var reader = await comando.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
        return null;

    return MapearUsuario(reader);
}

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_usuario,
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            FROM tb_usuario
            WHERE co_seq_usuario = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var reader = await comando.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapearUsuario(reader);
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM tb_usuario
                WHERE co_perfil = @idPerfil
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(
            comando,
            "@idPerfil",
            PerfilIds.Administrador
        );

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario (
                no_usuario,
                ds_email,
                ds_senha_hash,
                st_ativo,
                dt_cadastro,
                co_perfil
            )
            VALUES (
                @nome,
                @email,
                @senhaHash,
                @ativo,
                @dataCadastro,
                @idPerfil
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@dataCadastro", usuario.DataCadastro);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    private static Usuario MapearUsuario(DbDataReader reader)
    {
        return new Usuario
        {
            Id = reader.GetInt32(
                reader.GetOrdinal("co_seq_usuario")
            ),

            Nome = reader.GetString(
                reader.GetOrdinal("no_usuario")
            ),

            Email = reader.GetString(
                reader.GetOrdinal("ds_email")
            ),

            SenhaHash = reader.GetString(
                reader.GetOrdinal("ds_senha_hash")
            ),

            Ativo = reader.GetBoolean(
                reader.GetOrdinal("st_ativo")
            ),

            DataCadastro = reader.GetDateTime(
                reader.GetOrdinal("dt_cadastro")
            ),

            IdPerfil = Convert.ToInt32(
                reader["co_perfil"]
            )
        };
    }
}
