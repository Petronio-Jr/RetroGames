using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(
        IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    // GET: api/Plataforma
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas =
            await _plataformaService.ListarAsync();

        var response =
            new RetroGamesResponseApiModel<
                List<ListarPlataformaResponseApiModel>>
            {
                ResponseCode = ResponseCodes.SUCESSO.Code,
                Data = plataformas
                    .Select(plataforma => plataforma.ParaResponse())
                    .ToList(),
                ErrorMessage = null
            };

        return Ok(response);
    }

    // GET: api/Plataforma/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma =
            await _plataformaService.BuscarPorIdAsync(id);

        var response =
            new RetroGamesResponseApiModel<
                ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO.Code,
                Data = plataforma.ParaResponse(),
                ErrorMessage = null
            };

        return Ok(response);
    }
}
