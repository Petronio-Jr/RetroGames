using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using RetroGames.WebApi.ApiModels.Responses;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AddAutenticacao(
        this IServiceCollection services)
    {
        services
            .AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme
            )
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;

                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                // Retorna 401 sem redirecionar para uma tela de login.
                opcoes.Events.OnRedirectToLogin = contexto =>
                {
                    contexto.Response.StatusCode =
                        StatusCodes.Status401Unauthorized;

                    var erro = ResponseCodes.NAO_AUTENTICADO;

                    var response = new RetroGamesResponseApiModel
                    {
                        ResponseCode = erro.Code,
                        ErrorMessage = erro.DefaultMessage
                    };

                    return contexto.Response.WriteAsJsonAsync(response);
                };

                // Retorna 403 sem redirecionar para uma tela de acesso negado.
                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                {
                    contexto.Response.StatusCode =
                        StatusCodes.Status403Forbidden;

                    var erro = ResponseCodes.ACESSO_NEGADO;

                    var response = new RetroGamesResponseApiModel
                    {
                        ResponseCode = erro.Code,
                        ErrorMessage = erro.DefaultMessage
                    };

                    return contexto.Response.WriteAsJsonAsync(response);
                };
            });

        return services;
    }
}
