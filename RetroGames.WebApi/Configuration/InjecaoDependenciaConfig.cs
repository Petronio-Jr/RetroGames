using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using Microsoft.Extensions.DependencyInjection;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection services,
        string connectionString)
    {
        // Configuração do banco: uma única instância para a aplicação.
        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString
        });

        // Repositórios: uma instância por requisição.
        // Repositórios: uma instância por requisição.
        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();

// Repositórios do módulo de compras.
        services.AddScoped<ISituacaoPedidoRepository, SituacaoPedidoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IItemPedidoRepository, ItemPedidoRepository>();

        // Agregador dos repositórios.
        services.AddScoped<IRepositorySet, RepositorySet>();

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(
        this IServiceCollection services)
    {
        // Services: uma instância por requisição.
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        return services;
    }
}
