
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    // 1. Lista todas as plataformas.
    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _repositorio.Plataforma.ListarAsync();
    }

    // 2. Busca uma plataforma pelo ID.
    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNaoEncontrada);
        }

        return plataforma;
    }

    // 3. Insere uma nova plataforma.
    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        // Remove espaços do início e do fim do nome.
        plataforma.Nome = plataforma.Nome.Trim();

        // Verifica se o nome já está cadastrado.
        if (await _repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            // Grava e recebe o ID gerado pelo banco.
            plataforma.Id = await _repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            // Trata duplicidade causada por gravações simultâneas.
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }

        return plataforma;
    }

    // 4. Altera uma plataforma existente.
    public async Task<Plataforma> AlterarAsync(Plataforma plataforma)
    {
        // Verifica se a plataforma existe.
        await BuscarPorIdAsync(plataforma.Id);

        // Remove espaços do início e do fim do nome.
        plataforma.Nome = plataforma.Nome.Trim();

        // Ignora o próprio ID na verificação de duplicidade.
        if (await _repositorio.Plataforma.ExisteComNomeAsync(
            plataforma.Nome,
            plataforma.Id))
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            await _repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            // Trata duplicidade causada por gravações simultâneas.
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNomeDuplicado);
        }

        return plataforma;
    }

    // 5. Exclui uma plataforma sem jogos vinculados.
    public async Task ExcluirAsync(int id)
    {
        // Verifica se a plataforma existe.
        await BuscarPorIdAsync(id);

        // Verifica se existem jogos associados.
        if (await _repositorio.Plataforma.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaPossuiJogos);
        }

        // Exclui somente se não houver jogos vinculados.
        await _repositorio.Plataforma.ExcluirAsync(id);
    }
}
