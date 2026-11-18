
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    // GET: api/Jogo
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();

        var resultado = await _jogoService.ListarAsync(
            filtro.Busca,
            filtro.IdPlataforma,
            filtro.IdGenero,
            filtro.Pagina,
            filtro.TamanhoPagina
        );

        var pagina = new PaginaResponseApiModel<
            ListarJogoResponseApiModel>
        {
            Itens = resultado.Jogos
                .Select(jogo => jogo.ParaResponse())
                .ToList(),

            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalItens = resultado.Total,

            TotalPaginas = (int)Math.Ceiling(
                (double)resultado.Total / filtro.TamanhoPagina
            )
        };

        var response = new RetroGamesResponseApiModel<
            PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            Data = pagina,
            ErrorMessage = null
        };

        return Ok(response);
    }

    // GET: api/Jogo/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        var response = new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            Data = jogo.ParaDetalheResponse(),
            ErrorMessage = null
        };

        return Ok(response);
    }

    // POST: api/Jogo
    [HttpPost]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirJogoRequestApiModel request)
    {
        var criado = await _jogoService.InserirAsync(
            request.ParaModel(),
            request.IdsGeneros
        );

        var response = new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            Data = criado.ParaDetalheResponse(),
            ErrorMessage = null
        };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criado.Jogo.Id },
            response
        );
    }

    // PUT: api/Jogo/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(
        int id,
        [FromBody] AlterarJogoRequestApiModel request)
    {
        var alterado = await _jogoService.AlterarAsync(
            request.ParaModel(id),
            request.IdsGeneros
        );

        var response = new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            Data = alterado.ParaDetalheResponse(),
            ErrorMessage = null
        };

        return Ok(response);
    }

    // DELETE: api/Jogo/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _jogoService.DesativarAsync(id);

        var response = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            ErrorMessage = null
        };

        return Ok(response);
    }
}
