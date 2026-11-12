
using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    // 1. Lista jogos com filtros, paginação e detalhes.
    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca,
        int? idPlataforma,
        int? idGenero,
        int pagina,
        int tamanhoPagina)
    {
        List<int>? idsJogos = null;

        // Resolve o filtro por gênero no Service.
        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarAsync();

            idsJogos = vinculos
                .Where(v => v.GeneroId == idGenero.Value)
                .Select(v => v.JogoId)
                .Distinct()
                .ToList();
        }

        // A paginação e a contagem são feitas no Repository.
        // Jogos inativos não são incluídos nesta etapa.
        var resultado = await _repositorio.Jogo.ListarAsync(
            busca,
            idPlataforma,
            idsJogos,
            false,
            pagina,
            tamanhoPagina);

        var jogosDetalhados = await MontarDetalhesAsync(resultado.Jogos);

        return (jogosDetalhados, resultado.Total);
    }

    // 2. Busca um jogo pelo ID.
    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null || !jogo.Ativo)
        {
            throw new BusinessRuleException(
                BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync(
            new List<Jogo> { jogo });

        return detalhes[0];
    }

    // 3. Insere um jogo e seus gêneros em uma transação.
    public async Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        List<int> generosIds)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;

        await ValidarReferenciasAsync(jogo, generosIds);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            // Insere o jogo e recebe o ID gerado.
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);

            // Cria os vínculos usando o ID do jogo.
            var vinculos = generosIds
                .Distinct()
                .Select(idGenero => new JogoGenero
                {
                    JogoId = jogo.Id,
                    GeneroId = idGenero
                })
                .ToList();

            await _repositorio.JogoGenero.InserirAsync(vinculos);

            // Confirma somente depois da última gravação.
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    // 4. Altera um jogo e substitui seus gêneros.
    public async Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        List<int> generosIds)
    {
        // Confere se o jogo existe.
        var jogoExistente = await _repositorio.Jogo
            .BuscarPorIdAsync(jogo.Id);

        if (jogoExistente is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JogoNaoEncontrado);
        }

        jogo.Nome = jogo.Nome.Trim();

        await ValidarReferenciasAsync(jogo, generosIds);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            // Atualiza os dados do jogo.
            await _repositorio.Jogo.AlterarAsync(jogo);

            // Remove os vínculos anteriores.
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

            // Prepara os novos vínculos.
            var vinculos = generosIds
                .Distinct()
                .Select(idGenero => new JogoGenero
                {
                    JogoId = jogo.Id,
                    GeneroId = idGenero
                })
                .ToList();

            // Grava os novos vínculos.
            await _repositorio.JogoGenero.InserirAsync(vinculos);

            // Confirma somente depois da última gravação.
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    // 5. Desativa um jogo sem apagar seu registro.
    public async Task DesativarAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JogoNaoEncontrado);
        }

        await _repositorio.Jogo.DesativarAsync(id);
    }

    // Valida a existência das referências do jogo.
    private async Task ValidarReferenciasAsync(
        Jogo jogo,
        List<int> generosIds)
    {
        // Valida a plataforma.
        var plataforma = await _repositorio.Plataforma
            .BuscarPorIdAsync(jogo.PlataformaId);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PlataformaNaoEncontrada);
        }

        // Valida a condição.
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(c => c.Id == jogo.CondicaoJogoId))
        {
            throw new BusinessRuleException(
                BusinessErrors.CondicaoNaoEncontrada);
        }

        // Valida os gêneros.
        var generos = await _repositorio.Genero.ListarAsync();

        var idsValidos = generos
            .Select(g => g.Id)
            .ToHashSet();

        if (generosIds is null ||
            generosIds.Count == 0 ||
            generosIds.Distinct().Any(id => !idsValidos.Contains(id)))
        {
            throw new BusinessRuleException(
                BusinessErrors.GeneroNaoEncontrado);
        }
    }

    // Monta os detalhes de vários jogos utilizando LINQ.
    // Executa quatro consultas em lote, independentemente
    // da quantidade de jogos recebidos.
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var idsJogos = jogos
            .Select(j => j.Id)
            .ToList();

        // Consultas em lote.
        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();

        var vinculos = await _repositorio.JogoGenero
            .ListarPorJogosAsync(idsJogos);

        // Organiza as entidades por suas chaves primárias Id.
        var plataformasPorId = plataformas.ToDictionary(p => p.Id);
        var condicoesPorId = condicoes.ToDictionary(c => c.Id);

        // Combina os dados utilizando LINQ.
        return jogos.Select(jogo =>
        {
            var idsGenerosDoJogo = vinculos
                .Where(v => v.JogoId == jogo.Id)
                .Select(v => v.GeneroId)
                .ToHashSet();

            return new JogoDetalheServiceModel
            {
                Jogo = jogo,

                NomePlataforma = plataformasPorId.TryGetValue(
                    jogo.PlataformaId,
                    out var plataforma)
                    ? plataforma.Nome
                    : string.Empty,

                DescricaoCondicao = condicoesPorId.TryGetValue(
                    jogo.CondicaoJogoId,
                    out var condicao)
                    ? condicao.Descricao
                    : string.Empty,

                Generos = generos
                    .Where(g => idsGenerosDoJogo.Contains(g.Id))
                    .ToList()
            };
        }).ToList();
    }
}
