
using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A string de conexão 'Default' não foi configurada."
    );
}

// Registra os repositórios e serviços.
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

// Registra os validators do assembly da WebApi.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Registra as controllers e o filtro global.
builder.Services.AddControllers(opcoes =>
{
    opcoes.Filters.Add<FluentValidationFilter>();
});

// Configura o tratamento de erros do ModelState.
builder.Services.Configure<ApiBehaviorOptions>(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var erro = ResponseCodes.MODEL_STATE_INVALIDO;

        var response = new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tratamento global de exceções.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
