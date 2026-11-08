using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarAsync();

    Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos);

    Task InserirAsync(List<JogoGenero> vinculos);

    Task RemoverPorJogoAsync(int jogoId);
}