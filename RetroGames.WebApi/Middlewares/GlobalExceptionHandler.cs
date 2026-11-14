
using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Responses;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var response = new RetroGamesResponseApiModel();

        if (exception is BusinessRuleException)
        {
            var erro = ((BusinessRuleException)exception).Erro;

            response.ResponseCode = erro.Codigo;
            response.ErrorMessage = exception.Message;

            context.Response.StatusCode = (int)erro.StatusHttp;
        }
        else
        {
            _logger.LogError(
                exception,
                "Ocorreu um erro inesperado na API."
            );

            response.ResponseCode = ResponseCodes.ERRO.Code;
            response.ErrorMessage =
                ResponseCodes.ERRO.DefaultMessage;

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;
        }

        await context.Response.WriteAsJsonAsync(
            response,
            cancellationToken
        );

        return true;
    }
}
