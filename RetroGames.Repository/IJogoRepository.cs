using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        List<int>? idsJogos,
        bool incluirInativos,
        int pagina,
        int tamanhoPagina);

    Task<Jogo?> BuscarPorIdAsync(int id);

    Task<bool> PossuiJogosAsync(int idPlataforma);

    Task<int> InserirAsync(Jogo jogo);

    Task AlterarAsync(Jogo jogo);

    Task DesativarAsync(int id);
}
