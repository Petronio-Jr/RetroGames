using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorios;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacaoService(
        IRepositorySet repositorios,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _repositorios = repositorios;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(
        string email,
        string senha)
    {
        var usuario = await _repositorios.Usuario
            .BuscarPorEmailAsync(email.Trim());

        if (usuario is null || !usuario.Ativo)
            return null;

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha
        );

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException(
                "O e-mail do administrador inicial não foi configurado."
            );
        }

        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "A senha do administrador inicial não foi configurada."
            );
        }

        var existeAdministrador = await _repositorios.Usuario
            .ExisteAdministradorAsync();

        if (existeAdministrador)
            return;

        var administrador = new Usuario
        {
            Nome = "Administrador",
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador
        };

        administrador.SenhaHash = _passwordHasher.HashPassword(
            administrador,
            senha
        );

        await _repositorios.Usuario.InserirAsync(administrador);
    }
}
