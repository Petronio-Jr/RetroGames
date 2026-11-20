using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(
        this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            // Por padrão, toda rota exige autenticação.
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Política exclusiva para administradores.
            opcoes.AddPolicy(
                Politicas.Administrador,
                politica => politica.RequireRole(Perfis.Administrador)
            );
        });

        return services;
    }
}
