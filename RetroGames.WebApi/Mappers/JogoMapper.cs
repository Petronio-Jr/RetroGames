using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static (
        string? Busca,
        int? IdPlataforma,
        int? IdGenero,
        int Pagina,
        int TamanhoPagina
    ) ParaFiltro(this ListarJogoRequestApiModel request)
    {
        return (
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.Pagina,
            request.TamanhoPagina
        );
    }

    public static ListarJogoResponseApiModel ParaResponse(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Ano = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao,
            Ano = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            Generos = detalhe.Generos
                .Select(genero => new ListarGeneroResponseApiModel
                {
                    Id = genero.Id,
                    Nome = genero.Nome
                })
                .ToList()
        };
    }
}
