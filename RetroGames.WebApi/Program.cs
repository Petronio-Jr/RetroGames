using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using RetroGames.WebApi.Services;

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

// Repositórios e serviços de negócio.
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

// Autenticação e autorização.
builder.Services.AddAutenticacao();
builder.Services.AddAutorizacao();

// Permite acessar o usuário da requisição atual.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<
    IUsuarioAtualService,
    UsuarioAtualService
>();

// Validators.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Controllers e filtro global.
builder.Services.AddControllers(opcoes =>
{
    opcoes.Filters.Add<FluentValidationFilter>();
});

// Tratamento de erros do ModelState.
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

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Garante a existência do administrador inicial.
await using (var escopo = app.Services.CreateAsyncScope())
{
    var autenticacaoService = escopo.ServiceProvider
        .GetRequiredService<IAutenticacaoService>();

    var emailAdministrador =
        app.Configuration["AdminInicial:Email"];

    var senhaAdministrador =
        app.Configuration["AdminInicial:Senha"];

    await autenticacaoService.GarantirAdministradorInicialAsync(
        emailAdministrador ?? string.Empty,
        senhaAdministrador ?? string.Empty
    );
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // O Swagger permanece público.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Identifica o usuário pelo cookie.
app.UseAuthentication();

// Aplica as políticas de autorização.
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
