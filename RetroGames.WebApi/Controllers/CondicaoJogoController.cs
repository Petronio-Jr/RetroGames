using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _condicaoJogoService;

    public CondicaoJogoController(
        ICondicaoJogoService condicaoJogoService)
    {
        _condicaoJogoService = condicaoJogoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _condicaoJogoService.ListarAsync();

        var response = new RetroGamesResponseApiModel<
            List<ListarCondicaoJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            Data = condicoes
                .Select(condicao => condicao.ParaResponse())
                .ToList(),
            ErrorMessage = null
        };

        return Ok(response);
    }
}
