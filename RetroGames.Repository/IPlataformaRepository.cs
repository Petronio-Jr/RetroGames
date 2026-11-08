using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPlataformaRepository
{
    // Lista todas as plataformas
    Task<List<Plataforma>> ListarAsync();

    // Busca uma plataforma pelo ID
    Task<Plataforma?> BuscarPorIdAsync(int id);

    // Verifica se existe uma plataforma com o nome informado
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);

    // Verifica se a plataforma possui jogos
    Task<bool> PossuiJogosAsync(int id);

    // Insere uma nova plataforma e retorna o ID gerado
    Task<int> InserirAsync(Plataforma plataforma);

    // Altera uma plataforma existente
    Task AlterarAsync(Plataforma plataforma);

    // Exclui uma plataforma pelo ID
    Task ExcluirAsync(int id);
}