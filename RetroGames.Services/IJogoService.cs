
using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        int? idGenero,
        int pagina,
        int tamanhoPagina);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        List<int> generosIds);

    Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        List<int> generosIds);

    Task DesativarAsync(int id);
}
