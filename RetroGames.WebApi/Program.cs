using DotNetEnv;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

// Carrega as variáveis do arquivo .env
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Obtém a string de conexão
var connectionString =
    builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A string de conexão 'Default' não foi configurada."
    );
}

// Configuração das dependências
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tratamento global de exceções
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// O tratamento de exceções deve ser o primeiro middleware
app.UseExceptionHandler();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Demais middlewares
app.UseAuthorization();

// Rotas
app.MapControllers();

app.Run();

public partial class Program
{
}
