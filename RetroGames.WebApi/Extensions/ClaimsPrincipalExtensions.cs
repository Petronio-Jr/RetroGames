using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal CriarClaimsPrincipal(
        this Usuario usuario)
    {
        var perfil = usuario.IdPerfil == PerfilIds.Administrador
            ? Perfis.Administrador
            : Perfis.Cliente;

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()
            ),

            new(
                ClaimTypes.Name,
                usuario.Nome
            ),

            new(
                ClaimTypes.Email,
                usuario.Email
            ),

            new(
                ClaimTypes.Role,
                perfil
            )
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return new ClaimsPrincipal(identidade);
    }

    public static int? ObterIdUsuario(
        this ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirst(
            ClaimTypes.NameIdentifier
        )?.Value;

        if (int.TryParse(id, out var idUsuario))
            return idUsuario;

        return null;
    }

    public static string? ObterPerfil(
        this ClaimsPrincipal usuario)
    {
        return usuario.FindFirst(
            ClaimTypes.Role
        )?.Value;
    }
}
