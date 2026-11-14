
using DotNetEnv;
using RetroGames.WebApi.Configuration;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Lê a connection string do arquivo .env.
var connectionString =
    builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string 'Default' não foi encontrada no arquivo .env."
    );
}

// Registra repositórios e serviços.
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

// Registra controllers e Swagger.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger disponível em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

// Necessário para os testes automatizados.
public partial class Program { }
