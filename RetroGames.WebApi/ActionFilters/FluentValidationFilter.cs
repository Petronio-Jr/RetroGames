
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels.Responses;
using System.Text.Json;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    public int Order => -3000;

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // Verifica se algum parâmetro do corpo da requisição veio nulo.
        var corpoNulo = context.ActionDescriptor.Parameters
            .Where(parametro =>
                parametro.BindingInfo?.BindingSource == BindingSource.Body)
            .Any(parametro =>
                context.ActionArguments.TryGetValue(
                    parametro.Name,
                    out var valor) && valor == null);

        if (corpoNulo)
        {
            var erro = ResponseCodes.MODEL_NULL;

            context.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = erro.Code,
                    ErrorMessage = erro.DefaultMessage
                });

            return;
        }

        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento == null)
                continue;

            var tipoArgumento = argumento.GetType();

            // Busca os validators pela interface genérica correta.
            var tipoValidator = typeof(IValidator<>)
                .MakeGenericType(tipoArgumento);

            var validators = context.HttpContext.RequestServices
                .GetServices(tipoValidator);

            foreach (var validatorObjeto in validators)
            {
                var validator = (IValidator)validatorObjeto!;

                // Cria o contexto usando o tipo real do argumento.
                var tipoContexto = typeof(ValidationContext<>)
                    .MakeGenericType(tipoArgumento);

                var contextoValidacao =
                    (IValidationContext)Activator.CreateInstance(
                        tipoContexto,
                        argumento
                    )!;

                var resultado = await validator.ValidateAsync(
                    contextoValidacao,
                    context.HttpContext.RequestAborted
                );

                foreach (var falha in resultado.Errors)
                {
                    var nomeCampo = JsonNamingPolicy.CamelCase
                        .ConvertName(falha.PropertyName);

                    if (!erros.ContainsKey(nomeCampo))
                    {
                        erros[nomeCampo] = new List<string>();
                    }

                    erros[nomeCampo].Add(falha.ErrorMessage);
                }
            }
        }

        if (erros.Count > 0)
        {
            var erro = ResponseCodes.MODEL_STATE_INVALIDO;

            var data = erros.ToDictionary(
                item => item.Key,
                item => item.Value.ToArray()
            );

            context.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel<Dictionary<string, string[]>>
                {
                    ResponseCode = erro.Code,
                    Data = data,
                    ErrorMessage = erro.DefaultMessage
                }
            );

            return;
        }

        await next();
    }
}
